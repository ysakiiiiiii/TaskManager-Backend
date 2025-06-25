using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
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
                .Include(t => t.AssignedUsers)
                    .ThenInclude(ta => ta.User)
                .Include(t => t.CheckListItems)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            await dbContext.Tasks.AddAsync(task);
            await dbContext.SaveChangesAsync();

            return await dbContext.Tasks
                .Include(t => t.AssignedUsers)
                    .ThenInclude(ta => ta.User)
                .Include(t => t.CheckListItems)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == task.Id) ?? task;
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await dbContext.Tasks
                .Include(t => t.AssignedUsers)
                    .ThenInclude(ta => ta.User)
                .Include(t => t.CheckListItems)
                .Include(t => t.Attachments)
                .ToListAsync();
        }
    }
}
