using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategory()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(ApiResponse.SuccessResponse(categories));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(ApiResponse.SuccessResponse(category));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] AddCategoryRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid input", ModelState.ToErrorList()));

        var createdCategory = await _categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetCategoryById),
            new { id = createdCategory.Id },
            ApiResponse.SuccessResponse(createdCategory, "Category created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] UpdateCategoryRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid input", ModelState.ToErrorList()));

        var updatedCategory = await _categoryService.UpdateCategoryAsync(id, dto);
        return Ok(ApiResponse.SuccessResponse(updatedCategory, "Category updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return Ok(ApiResponse.SuccessResponse(null, "Category deleted successfully"));
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetSearchFilters()
    {
        var searchFilters = await _categoryService.GetSearchFiltersAsync();
        return Ok(ApiResponse.SuccessResponse(searchFilters, "Search filters fetched successfully"));
    }
}
