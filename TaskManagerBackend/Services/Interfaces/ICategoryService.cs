using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(AddCategoryRequestDto categoryRequestDto);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryRequestDto categoryRequestDto);
        Task DeleteCategoryAsync(int id);
        Task ReassignTasksAndDeleteCategoryAsync(int oldCategoryId, int newCategoryId);

        Task<SearchFiltersDto> GetSearchFiltersAsync();
    }

}
