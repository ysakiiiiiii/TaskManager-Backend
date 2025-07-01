using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategory()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse.SuccessResponse(categories));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving categories"));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Category not found"));
            }

            return Ok(ApiResponse.SuccessResponse(category));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting category with ID {id}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving the category"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] AddCategoryRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));
            }

            if (dto == null)
            {
                return BadRequest(ApiResponse.ErrorResponse("Category data cannot be null"));
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetAllCategory),
                new { id = createdCategory.Id },
                ApiResponse.SuccessResponse(createdCategory, "Category created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while creating the category"));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] UpdateCategoryRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));
            }

            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, dto);
            if (updatedCategory == null || updatedCategory.Id == 0)
            {
                return NotFound(ApiResponse.ErrorResponse("Category not found or update failed"));
            }

            return Ok(ApiResponse.SuccessResponse(updatedCategory, "Category updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating category with ID {id}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while updating the category"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse("Category not found or deletion failed"));
            }

            return Ok(ApiResponse.SuccessResponse(null, "Category deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting category with ID {id}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the category"));
        }
    }
}