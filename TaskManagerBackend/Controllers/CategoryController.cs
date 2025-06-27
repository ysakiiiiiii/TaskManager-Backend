using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.Services;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await categoryService.GetCategoryByIdAsync(id);
            if (task == null)
                return NotFound(new { Message = "Task not found." });

            return Ok(task);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] AddCategoryRequestDto categoryRequestDto)
        {
            if (categoryRequestDto != null)
            {
                var createdCategory = await categoryService.CreateCategoryAsync(categoryRequestDto);
                return CreatedAtAction(nameof(GetAllCategory), new { id = createdCategory.Id }, createdCategory);
            }

            return BadRequest("Invalid category data provided.");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            var updatedCategory = await categoryService.UpdateCategoryAsync(id, updateCategoryRequestDto);
            if (updatedCategory == null || updatedCategory.Id == 0)
            {
                return NotFound(new { Message = "Category not found or update failed." });
            }

            return Ok(updatedCategory);

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            var result = await categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound(new { Message = "Category not found or deletion failed." });
            }
            return Ok(new { Message = "Category deleted successfully." });
        }
    }
}
                