using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext dbContext;

        public TaskRepository(TaskDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await dbContext.Tasks
                .Include(t => t.AssignedUsers).ThenInclude(ta => ta.User)
                .Include(t => t.CheckListItems)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await dbContext.Tasks
                .Include(t => t.AssignedUsers).ThenInclude(ta => ta.User)
                .Include(t => t.CheckListItems)
                .Include(t => t.Attachments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            await dbContext.Tasks.AddAsync(task);
            await dbContext.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
        {
            var existingTask = await dbContext.Tasks.FindAsync(task.Id);
            if (existingTask == null) return null;

            dbContext.Entry(existingTask).CurrentValues.SetValues(task);
            await dbContext.SaveChangesAsync();
            return existingTask;
        }

        public async Task<bool?> DeleteTaskAsync(TaskItem task)
        {
            var existingTask = await dbContext.Tasks.FindAsync(task.Id);
            if (existingTask == null) return null;

            dbContext.Tasks.Remove(existingTask);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}