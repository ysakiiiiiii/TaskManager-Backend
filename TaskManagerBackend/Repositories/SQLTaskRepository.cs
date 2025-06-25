using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public class SQLTaskRepository : ITaskRepository
    {
        private readonly TaskDbContext dbContext;

        public SQLTaskRepository(TaskDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<TaskItem> CreateAsync(TaskItem taskItem)
        {
            await dbContext.Tasks.AddAsync(taskItem);
            await dbContext.SaveChangesAsync();
            return taskItem;            
        }
    }
}
