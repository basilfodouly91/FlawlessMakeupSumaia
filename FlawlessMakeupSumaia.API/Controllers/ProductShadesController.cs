using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/[controller]")]
    public class ProductShadesController : ControllerBase
    {
        private readonly IProductShadeService _shadeService;

        public ProductShadesController(IProductShadeService shadeService)
        {
            _shadeService = shadeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductShadeDto>>> GetShadesByProduct(int productId)
        {
            var shades = await _shadeService.GetShadesByProductIdAsync(productId);
            return Ok(shades.Select(s => s.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductShadeDto>> GetShade(int id)
        {
            var shade = await _shadeService.GetShadeByIdAsync(id);
            if (shade == null)
                return NotFound();

            return Ok(shade.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductShadeDto>> CreateShade(int productId, CreateProductShadeDto dto)
        {
            var shade = dto.ToModel(productId);
            var createdShade = await _shadeService.CreateShadeAsync(productId, shade);
            return CreatedAtAction(nameof(GetShade), new { productId, id = createdShade.Id }, createdShade.ToDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductShadeDto>> UpdateShade(int id, UpdateProductShadeDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            try
            {
                var shade = await _shadeService.GetShadeByIdAsync(id);
                if (shade == null)
                    return NotFound();

                shade.Name = dto.Name;
                shade.StockQuantity = dto.StockQuantity;
                shade.IsActive = dto.IsActive;
                shade.DisplayOrder = dto.DisplayOrder;

                var updatedShade = await _shadeService.UpdateShadeAsync(shade);
                return Ok(updatedShade.ToDto());
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShade(int id)
        {
            var result = await _shadeService.DeleteShadeAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}










