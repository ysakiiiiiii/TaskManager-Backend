using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TaskDbContext dbContext;

        public CategoryRepository(TaskDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id) => await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<Category>> GetAllCategoryAsync() => await dbContext.Categories.ToListAsync();

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
            var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return null;
            dbContext.Remove(category);
            await dbContext.SaveChangesAsync();
            return category;
        }
    }
}
