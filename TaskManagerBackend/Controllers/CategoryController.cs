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

    }
}
                