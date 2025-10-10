using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FlawlessMakeupSumaia.API.Services;
using FlawlessMakeupSumaia.API.DTOs;

namespace FlawlessMakeupSumaia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories.Select(c => c.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category.ToDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            var category = dto.ToModel();
            var createdCategory = await _categoryService.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory.ToDto());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
            if (existingCategory == null)
                return NotFound();

            // Update properties
            existingCategory.Name = dto.Name;
            existingCategory.Description = dto.Description;
            existingCategory.ImageUrl = dto.ImageUrl;
            existingCategory.DisplayOrder = dto.DisplayOrder;
            existingCategory.IsActive = dto.IsActive;

            var updatedCategory = await _categoryService.UpdateCategoryAsync(existingCategory);
            return Ok(updatedCategory.ToDto());
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
