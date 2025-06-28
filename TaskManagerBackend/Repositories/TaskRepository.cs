using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext dbContext;

    public TaskRepository(TaskDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id) =>
        await dbContext.Tasks
            .Include(t => t.AssignedUsers).ThenInclude(ta => ta.User)
            .Include(t => t.CheckListItems)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<List<TaskItem>> GetAllTasksAsync() =>
        await dbContext.Tasks
            .Include(t => t.AssignedUsers).ThenInclude(ta => ta.User)
            .Include(t => t.CheckListItems)
            .Include(t => t.Attachments)
            .ToListAsync();

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        await dbContext.Tasks.AddAsync(task);
        await dbContext.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
    {
        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync();
        return task;
    }

    public async Task<bool?> DeleteTaskAsync(TaskItem task)
    {
        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
