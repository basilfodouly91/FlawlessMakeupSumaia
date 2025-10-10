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
            var product = dto.ToModel();
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct.ToDto());
        }

        [HttpPut("{id}")]
        // [Authorize] // Temporarily disabled for testing
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto dto)
        {
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

            var updatedProduct = await _productService.UpdateProductAsync(existingProduct);
            return Ok(updatedProduct.ToDto());
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
