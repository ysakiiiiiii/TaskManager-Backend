using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategoryAsync(Category category);
        Task<List<Category>> GetAllCategoryAsync();
        Task<Category> UpdateCategoryAsync(int id, Category category);
    }
}
