using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Repositories.Implementations
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

        public async Task<List<string>> GetAllCategoryNamesAsync()
        {
            return await dbContext.Categories
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetAllPriorityNamesAsync()
        {
            return await dbContext.Priorities
                .Select(p => p.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetAllStatusNamesAsync()
        {
            return await dbContext.Statuses
                .Select(s => s.Name)
                .Distinct()
                .ToListAsync();
        }
    }
}
