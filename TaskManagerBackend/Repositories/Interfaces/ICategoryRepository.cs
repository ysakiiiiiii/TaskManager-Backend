using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<List<Category>> GetAllCategoryAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int id, Category category);
        Task<Category> DeleteCategoryAsync(int id);
        Task<List<string>> GetAllCategoryNamesAsync();
        Task<List<string>> GetAllPriorityNamesAsync();
        Task<List<string>> GetAllStatusNamesAsync();

    }
}
