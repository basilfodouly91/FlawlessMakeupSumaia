using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products.Select(p => p.ToDto()));
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts()
        {
            var products = await _productService.GetFeaturedProductsAsync();
            return Ok(products.Select(p => p.ToDto()));
        }

        [HttpGet("on-sale")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsOnSale()
        {
            var products = await _productService.GetProductsOnSaleAsync();
            return Ok(products.Select(p => p.ToDto()));
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products.Select(p => p.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product.ToDto());
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Search term is required");

            var products = await _productService.SearchProductsAsync(q);
            return Ok(products.Select(p => p.ToDto()));
        }

        [HttpPost]
        // [Authorize] // Temporarily disabled for testing
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
        {
            Console.WriteLine("=== CREATE PRODUCT REQUEST ===");
            Console.WriteLine($"Product Name: {dto.Name}");
            Console.WriteLine($"ProductShades in DTO: {dto.ProductShades?.Count ?? 0}");
            if (dto.ProductShades != null && dto.ProductShades.Any())
            {
                Console.WriteLine("ProductShades details:");
                foreach (var shade in dto.ProductShades)
                {
                    Console.WriteLine($"  - Name: {shade.Name}, Stock: {shade.StockQuantity}, Active: {shade.IsActive}, Order: {shade.DisplayOrder}");
                }
            }
            else
            {
                Console.WriteLine("WARNING: No ProductShades in DTO!");
            }
            
            var product = dto.ToModel();
            Console.WriteLine($"After ToModel, Product has {product.ProductShades?.Count ?? 0} shades");
            
            var createdProduct = await _productService.CreateProductAsync(product);
            Console.WriteLine("=== END CREATE PRODUCT REQUEST ===");
            
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct.ToDto());
        }

        [HttpPut("{id}")]
        // [Authorize] // Temporarily disabled for testing
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto dto)
        {
            try
            {
                Console.WriteLine($"=== UPDATE PRODUCT {id} ===");
                Console.WriteLine($"Received DTO: Name={dto.Name}, CategoryId={dto.CategoryId}");
                Console.WriteLine($"ProductShades is null: {dto.ProductShades == null}");
                Console.WriteLine($"ProductShades count: {dto.ProductShades?.Count ?? 0}");
                if (dto.ProductShades != null && dto.ProductShades.Any())
                {
                    Console.WriteLine($"ProductShades received:");
                    foreach (var shade in dto.ProductShades)
                    {
                        Console.WriteLine($"  Shade: Id={shade.Id}, Name={shade.Name}, Stock={shade.StockQuantity}, Active={shade.IsActive}, Order={shade.DisplayOrder}");
                    }
                }
                else
                {
                    Console.WriteLine("WARNING: No ProductShades received in DTO!");
                }

                if (id != dto.Id)
                    return BadRequest();

                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                    return NotFound();

                // Update properties
                existingProduct.Name = dto.Name;
                existingProduct.Description = dto.Description;
                existingProduct.Price = dto.Price;
                existingProduct.SalePrice = dto.SalePrice;
                existingProduct.StockQuantity = dto.StockQuantity;
                existingProduct.ImageUrl = dto.ImageUrl;
                existingProduct.ImageUrls = dto.ImageUrls;
                existingProduct.CategoryId = dto.CategoryId;
                existingProduct.IsFeatured = dto.IsFeatured;
                existingProduct.IsOnSale = dto.IsOnSale;
                existingProduct.IsActive = dto.IsActive;
                existingProduct.Brand = dto.Brand;
                existingProduct.Shade = dto.Shade;
                existingProduct.Size = dto.Size;
                existingProduct.Ingredients = dto.Ingredients;
                existingProduct.SkinType = dto.SkinType;

                // DON'T set ProductShades here - let the service handle them
                // We need to pass the shades to the service separately to avoid EF tracking issues
                
                // Map the DTO shades to model shades for the service
                var shadesToUpdate = dto.ProductShades?.Select(shadeDto => new Models.ProductShade
                {
                    Id = shadeDto.Id ?? 0,
                    Name = shadeDto.Name,
                    StockQuantity = shadeDto.StockQuantity,
                    IsActive = shadeDto.IsActive,
                    DisplayOrder = shadeDto.DisplayOrder
                }).ToList() ?? new List<Models.ProductShade>();

                // Set the shades on the product object that will be passed to the service
                // but use a new product instance to avoid EF tracking conflicts
                var productToUpdate = new Models.Product
                {
                    Id = existingProduct.Id,
                    Name = existingProduct.Name,
                    Description = existingProduct.Description,
                    Price = existingProduct.Price,
                    SalePrice = existingProduct.SalePrice,
                    StockQuantity = existingProduct.StockQuantity,
                    ImageUrl = existingProduct.ImageUrl,
                    ImageUrls = existingProduct.ImageUrls,
                    CategoryId = existingProduct.CategoryId,
                    IsFeatured = existingProduct.IsFeatured,
                    IsOnSale = existingProduct.IsOnSale,
                    IsActive = existingProduct.IsActive,
                    Brand = existingProduct.Brand,
                    Shade = existingProduct.Shade,
                    Size = existingProduct.Size,
                    Ingredients = existingProduct.Ingredients,
                    SkinType = existingProduct.SkinType,
                    ProductShades = shadesToUpdate
                };

                Console.WriteLine($"Calling UpdateProductAsync with {productToUpdate.ProductShades.Count} shades");
                var updatedProduct = await _productService.UpdateProductAsync(productToUpdate);
                Console.WriteLine("Product updated successfully!");
                return Ok(updatedProduct.ToDto());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in UpdateProduct controller: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{id}")]
        // [Authorize] // Temporarily disabled for testing
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
