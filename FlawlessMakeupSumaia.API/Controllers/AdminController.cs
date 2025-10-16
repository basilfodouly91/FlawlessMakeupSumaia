using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;
using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Temporarily disabled for testing - enable in production
    public class AdminController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;

        public AdminController(
            IProductService productService,
            ICategoryService categoryService,
            IOrderService orderService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _orderService = orderService;
        }

        // Dashboard Statistics
        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
        {
            var allProducts = await _productService.GetAllProductsAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();
            
            var dashboard = new AdminDashboardDto
            {
                TotalProducts = allProducts.Count(),
                ActiveProducts = allProducts.Count(p => p.IsActive),
                FeaturedProducts = allProducts.Count(p => p.IsFeatured),
                ProductsOnSale = allProducts.Count(p => p.IsOnSale),
                TotalCategories = categories.Count(),
                ActiveCategories = categories.Count(c => c.IsActive),
                LowStockProducts = allProducts.Count(p => p.StockQuantity < 10),
                OutOfStockProducts = allProducts.Count(p => p.StockQuantity == 0)
            };

            return Ok(dashboard);
        }

        // Product Management
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<AdminProductDto>>> GetAllProductsForAdmin()
        {
            var products = await _productService.GetAllProductsAsync();
            var adminProducts = products.Select(p => new AdminProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                SalePrice = p.SalePrice,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.NameEn : "",
                Brand = p.Brand,
                ProductShades = p.ProductShades.Select(ps => ps.ToDto()).ToList(),
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                IsOnSale = p.IsOnSale,
                DateCreated = p.DateCreated,
                DateUpdated = p.DateUpdated,
                Status = p.StockQuantity == 0 ? "Out of Stock" : 
                        p.StockQuantity < 10 ? "Low Stock" : "In Stock"
            });

            return Ok(adminProducts);
        }

        [HttpPost("products/bulk-update")]
        public async Task<ActionResult> BulkUpdateProducts(BulkUpdateDto bulkUpdate)
        {
            foreach (var productId in bulkUpdate.ProductIds)
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product != null)
                {
                    switch (bulkUpdate.Action.ToLower())
                    {
                        case "activate":
                            product.IsActive = true;
                            break;
                        case "deactivate":
                            product.IsActive = false;
                            break;
                        case "feature":
                            product.IsFeatured = true;
                            break;
                        case "unfeature":
                            product.IsFeatured = false;
                            break;
                        case "sale":
                            product.IsOnSale = true;
                            if (bulkUpdate.SalePrice.HasValue)
                                product.SalePrice = bulkUpdate.SalePrice.Value;
                            break;
                        case "removesale":
                            product.IsOnSale = false;
                            product.SalePrice = null;
                            break;
                    }
                    await _productService.UpdateProductAsync(product);
                }
            }

            return Ok(new { message = $"Bulk update completed for {bulkUpdate.ProductIds.Count} products" });
        }

        [HttpPost("products/{id}/toggle-sale")]
        public async Task<ActionResult<ProductDto>> ToggleProductSale(int id, [FromBody] SaleToggleDto saleData)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            product.IsOnSale = saleData.IsOnSale;
            product.SalePrice = saleData.IsOnSale ? saleData.SalePrice : null;

            var updatedProduct = await _productService.UpdateProductAsync(product);
            return Ok(updatedProduct.ToDto());
        }

        [HttpPost("products/{id}/toggle-featured")]
        public async Task<ActionResult<ProductDto>> ToggleProductFeatured(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            product.IsFeatured = !product.IsFeatured;
            var updatedProduct = await _productService.UpdateProductAsync(product);
            return Ok(updatedProduct.ToDto());
        }

        [HttpPost("products/{id}/toggle-active")]
        public async Task<ActionResult<ProductDto>> ToggleProductActive(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            product.IsActive = !product.IsActive;
            var updatedProduct = await _productService.UpdateProductAsync(product);
            return Ok(updatedProduct.ToDto());
        }

        [HttpPut("products/{id}/stock")]
        public async Task<ActionResult<ProductDto>> UpdateStock(int id, [FromBody] StockUpdateDto stockUpdate)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            product.StockQuantity = stockUpdate.Quantity;
            var updatedProduct = await _productService.UpdateProductAsync(product);
            return Ok(updatedProduct.ToDto());
        }

        // Category Management
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<AdminCategoryDto>>> GetAllCategoriesForAdmin()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var adminCategories = categories.Select(c => new AdminCategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                DateCreated = c.DateCreated,
                ProductCount = c.Products?.Count ?? 0
            });

            return Ok(adminCategories);
        }

        [HttpPost("categories/{id}/toggle-active")]
        public async Task<ActionResult<CategoryDto>> ToggleCategoryActive(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            category.IsActive = !category.IsActive;
            var updatedCategory = await _categoryService.UpdateCategoryAsync(category);
            return Ok(updatedCategory.ToDto());
        }

        // Analytics
        [HttpGet("analytics/products")]
        public async Task<ActionResult<ProductAnalyticsDto>> GetProductAnalytics()
        {
            var products = await _productService.GetAllProductsAsync();
            var analytics = new ProductAnalyticsDto
            {
                TotalProducts = products.Count(),
                ProductsByCategory = products.GroupBy(p => p.Category != null ? p.Category.NameEn : "")
                    .Select(g => new CategoryProductCount 
                    { 
                        CategoryName = g.Key, 
                        Count = g.Count() 
                    }).ToList(),
                ProductsByBrand = products.Where(p => !string.IsNullOrEmpty(p.Brand))
                    .GroupBy(p => p.Brand!)
                    .Select(g => new BrandProductCount 
                    { 
                        BrandName = g.Key, 
                        Count = g.Count() 
                    }).ToList(),
                StockStatus = new StockStatusDto
                {
                    InStock = products.Count(p => p.StockQuantity > 10),
                    LowStock = products.Count(p => p.StockQuantity > 0 && p.StockQuantity <= 10),
                    OutOfStock = products.Count(p => p.StockQuantity == 0)
                }
            };

            return Ok(analytics);
        }
    }

    // Admin DTOs
    public class AdminDashboardDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int FeaturedProducts { get; set; }
        public int ProductsOnSale { get; set; }
        public int TotalCategories { get; set; }
        public int ActiveCategories { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
    }

    public class AdminProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public List<ProductShadeDto> ProductShades { get; set; } = new List<ProductShadeDto>();
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AdminCategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public int ProductCount { get; set; }
    }

    public class BulkUpdateDto
    {
        public List<int> ProductIds { get; set; } = new List<int>();
        public string Action { get; set; } = string.Empty;
        public decimal? SalePrice { get; set; }
    }

    public class SaleToggleDto
    {
        public bool IsOnSale { get; set; }
        public decimal? SalePrice { get; set; }
    }

    public class StockUpdateDto
    {
        public int Quantity { get; set; }
    }

    public class ProductAnalyticsDto
    {
        public int TotalProducts { get; set; }
        public List<CategoryProductCount> ProductsByCategory { get; set; } = new List<CategoryProductCount>();
        public List<BrandProductCount> ProductsByBrand { get; set; } = new List<BrandProductCount>();
        public StockStatusDto StockStatus { get; set; } = new StockStatusDto();
    }

    public class CategoryProductCount
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class BrandProductCount
    {
        public string BrandName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class StockStatusDto
    {
        public int InStock { get; set; }
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
    }
}
