using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<List<Category>> GetAllCategoryAsync();
        Task ReassignTasksToCategoryAsync(int oldCategoryId, int newCategoryId);

        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int id, Category category);

        Task<Category> DeleteCategoryAsync(int id);
        Task<List<BasicDto>> GetAllCategoriesWithIdAsync();
        Task<List<BasicDto>> GetAllPrioritiesWithIdAsync();
        Task<List<BasicDto>> GetAllStatusesWithIdAsync();

    }
}
