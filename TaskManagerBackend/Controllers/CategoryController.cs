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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody]AddCategoryRequestDto categoryRequestDto)
        {
            if (categoryRequestDto != null)
            {
                var createdCategory  = await categoryService.CreateCategoryAsync(categoryRequestDto);
                return Ok("Successfully created category");
            }

            return BadRequest("Invalid category data provided.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute]int id, [FromBody]UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            var updatedCategory = await categoryService.UpdateCategoryAsync(id, updateCategoryRequestDto);
            if(updatedCategory == null || updatedCategory.Id == 0)
            {
                return NotFound(new { Message = "Category not found or update failed." });
            }

            return Ok(updatedCategory);

        }

    }
}
                