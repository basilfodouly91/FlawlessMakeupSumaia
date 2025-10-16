using Microsoft.EntityFrameworkCore;
using FlawlessMakeupSumaia.API.Data;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public ProductService(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductShades)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductShades)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsOnSaleAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductShades)
                .Where(p => p.IsActive && p.IsOnSale && p.SalePrice.HasValue)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductShades)
                .Where(p => p.IsActive && p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductShades)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            Console.WriteLine("=== CREATE PRODUCT ===");
            Console.WriteLine($"Product Name: {product.Name}");
            Console.WriteLine($"ProductShades count: {product.ProductShades?.Count ?? 0}");
            
            product.DateCreated = DateTime.UtcNow;
            product.DateUpdated = DateTime.UtcNow;
            
            // Process main image
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                product.ImageUrl = _imageService.ProcessImage(product.ImageUrl);
            }
            
            // Process additional images
            if (product.ImageUrls != null && product.ImageUrls.Count > 0)
            {
                product.ImageUrls = product.ImageUrls
                    .Select(img => _imageService.ProcessImage(img))
                    .ToList();
            }
            
            // Process ProductShades - ensure they have proper timestamps
            if (product.ProductShades != null && product.ProductShades.Any())
            {
                Console.WriteLine($"Processing {product.ProductShades.Count} shades:");
                foreach (var shade in product.ProductShades)
                {
                    Console.WriteLine($"  - Shade: {shade.Name}, Stock: {shade.StockQuantity}, Active: {shade.IsActive}");
                    shade.DateCreated = DateTime.UtcNow;
                    shade.DateUpdated = DateTime.UtcNow;
                }
            }
            else
            {
                Console.WriteLine("No product shades to process");
            }
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Product created with ID: {product.Id}");
            
            // Reload the product with all relationships to confirm shades were saved
            var createdProduct = await _context.Products
                .Include(p => p.ProductShades)
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == product.Id);
            
            Console.WriteLine($"Reloaded product has {createdProduct.ProductShades.Count} shades");
            Console.WriteLine("=== END CREATE PRODUCT ===");
            
            return createdProduct;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            try
            {
                product.DateUpdated = DateTime.UtcNow;
                
                // Process main image
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    product.ImageUrl = _imageService.ProcessImage(product.ImageUrl);
                }
                
                // Process additional images
                if (product.ImageUrls != null && product.ImageUrls.Count > 0)
                {
                    product.ImageUrls = product.ImageUrls
                        .Select(img => _imageService.ProcessImage(img))
                        .ToList();
                }

                // Get existing product WITHOUT shades (to avoid collection tracking conflicts)
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == product.Id);

                if (existingProduct == null)
                {
                    throw new ArgumentException("Product not found");
                }

                // Update product properties
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.SalePrice = product.SalePrice;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.ImageUrls = product.ImageUrls;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.IsFeatured = product.IsFeatured;
                existingProduct.IsOnSale = product.IsOnSale;
                existingProduct.IsActive = product.IsActive;
                existingProduct.Brand = product.Brand;
                existingProduct.Shade = product.Shade;
                existingProduct.Size = product.Size;
                existingProduct.Ingredients = product.Ingredients;
                existingProduct.SkinType = product.SkinType;
                existingProduct.DateUpdated = DateTime.UtcNow;

                // Handle ProductShades BEFORE saving - Complete replacement strategy
                // Store the shades from the incoming product parameter before any SaveChanges
                var incomingShades = product.ProductShades?.ToList() ?? new List<ProductShade>();
                Console.WriteLine($"Incoming product has {incomingShades.Count} shades");
                
                // Delete all existing shades for this product
                var existingShadesToDelete = await _context.ProductShades
                    .Where(ps => ps.ProductId == product.Id)
                    .ToListAsync();
                
                if (existingShadesToDelete.Any())
                {
                    Console.WriteLine($"Deleting {existingShadesToDelete.Count} existing shades");
                    _context.ProductShades.RemoveRange(existingShadesToDelete);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Existing shades deleted successfully");
                }
                
                // Save product property updates
                await _context.SaveChangesAsync();
                Console.WriteLine("Product properties updated successfully");
                
                // Now add new shades from the stored incomingShades list
                if (incomingShades.Any())
                {
                    Console.WriteLine($"Adding {incomingShades.Count} new shades to database:");
                    foreach (var shade in incomingShades)
                    {
                        Console.WriteLine($"  - Name={shade.Name}, Stock={shade.StockQuantity}, Active={shade.IsActive}");
                        var newShade = new ProductShade
                        {
                            ProductId = product.Id,
                            Name = shade.Name,
                            StockQuantity = shade.StockQuantity,
                            IsActive = shade.IsActive,
                            DisplayOrder = shade.DisplayOrder,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow
                        };
                        _context.ProductShades.Add(newShade);
                    }
                    
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Successfully saved {incomingShades.Count} shades to database!");
                }
                else
                {
                    Console.WriteLine("No shades to add (product has no shades)");
                }
                
                // Reload product with all relationships
                var updatedProduct = await _context.Products
                    .Include(p => p.ProductShades)
                    .Include(p => p.Category)
                    .FirstAsync(p => p.Id == product.Id);
                    
                Console.WriteLine($"Final product has {updatedProduct.ProductShades.Count} shades");
                return updatedProduct;
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error updating product: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            // Soft delete
            product.IsActive = false;
            product.DateUpdated = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && 
                    (p.Name.Contains(searchTerm) || 
                     p.Description.Contains(searchTerm) ||
                     p.Brand!.Contains(searchTerm) ||
                     p.Category.NameEn.Contains(searchTerm)))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
