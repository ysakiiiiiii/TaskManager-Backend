using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.Exceptions;
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
            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return null;

            try
            {
                dbContext.Categories.Remove(category);
                await dbContext.SaveChangesAsync();
                return category;
            }
            catch (DbUpdateException ex)
            {
                throw new BadRequestException("Cannot delete category. It is being used in existing tasks.");
            }
        }


        public async Task ReassignTasksToCategoryAsync(int oldCategoryId, int newCategoryId)
        {
            var tasksToUpdate = await dbContext.Tasks
                .Where(t => t.CategoryId == oldCategoryId)
                .ToListAsync();

            foreach (var task in tasksToUpdate)
            {
                task.CategoryId = newCategoryId;
            }

            dbContext.Tasks.UpdateRange(tasksToUpdate);
            await dbContext.SaveChangesAsync();
        }


        public async Task<List<BasicDto>> GetAllCategoriesWithIdAsync()
        {
            return await dbContext.Categories
                .Select(c => new BasicDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }

        public async Task<List<BasicDto>> GetAllPrioritiesWithIdAsync()
        {
            return await dbContext.Priorities
                .Select(p => new BasicDto { Id = p.Id, Name = p.Name })
                .ToListAsync();
        }

        public async Task<List<BasicDto>> GetAllStatusesWithIdAsync()
        {
            return await dbContext.Statuses
                .Select(s => new BasicDto { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }

    }
}
