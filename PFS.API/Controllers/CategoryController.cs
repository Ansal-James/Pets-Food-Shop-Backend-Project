using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.Application.DTOs.Category;
using PFS.Application.Interface;
using PFS.Infrastructure.Services;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _category;
        public CategoryController(ICategoryService category)
        {
            _category = category;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var result = await _category.CreateCategoryAsync(dto);
            return Ok(result);
        }

        // Update Category (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _category.UpdateCategoryAsync(id, dto);
            return Ok(result);
        }

        // Delete Category (Admin only) - Soft delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _category.DeleteCategoryAsync(id);
            return Ok(new { message = "Category deleted successfully" });
        }

        // ================= PUBLIC ENDPOINTS =================

        // Get all categories (User & Admin)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _category.GetAllCategoriesAsync();
            return Ok(result);
        }

        // Get category by id (User & Admin)
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _category.GetCategoryByIdAsync(id);
            return Ok(result);
        }
    }
}
