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

        public async Task<IQueryable<TaskItem>> GetFilteredTasksQueryAsync(
            string? search,
            string? category,
            string? priority,
            string? status,
            string? sortBy,
            bool isAscending,
            string? userId,
            string? userRole,
            string? type)
        {
            var query = dbContext.Tasks
                .Include(t => t.AssignedUsers).ThenInclude(ta => ta.User)
                .Include(t => t.CheckListItems)
                .Include(t => t.Attachments)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.CreatedBy)
                .AsQueryable();

            if (userRole?.ToLower() != "admin")
            {
                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "created")
                    {
                        query = query.Where(t => t.CreatedById == userId);
                    }
                    else if (type == "assigned")
                    {
                        query = query.Where(t => t.AssignedUsers.Any(a => a.UserId == userId));
                    }
                }
                else
                {
                    query = query.Where(t =>
                        t.CreatedById == userId ||
                        t.AssignedUsers.Any(a => a.UserId == userId));
                }
            }


            if (!string.IsNullOrEmpty(type))
            {
                if (type == "created")
                    query = query.Where(t => t.CreatedById == userId);
                else if (type == "assigned")
                    query = query.Where(t => t.AssignedUsers.Any(a => a.UserId == userId));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    t.AssignedUsers.Any(au =>
                        (au.User.FirstName + " " + au.User.LastName).Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(t => t.Category.Name == category);

            if (!string.IsNullOrWhiteSpace(priority))
                query = query.Where(t => t.Priority.Name == priority);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status.Name == status);

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "title" => isAscending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                    "duedate" => isAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
                    "priority" => isAscending ? query.OrderBy(t => t.Priority.Name) : query.OrderByDescending(t => t.Priority.Name),
                    "status" => isAscending ? query.OrderBy(t => t.Status.Name) : query.OrderByDescending(t => t.Status.Name),
                    "category" => isAscending ? query.OrderBy(t => t.Category.Name) : query.OrderByDescending(t => t.Category.Name),
                    "datecreated" => isAscending ? query.OrderBy(t => t.DateCreated) : query.OrderByDescending(t => t.DateCreated),
                    "createdby" => isAscending
                        ? query.OrderBy(t => t.CreatedBy.FirstName + " " + t.CreatedBy.LastName)
                        : query.OrderByDescending(t => t.CreatedBy.FirstName + " " + t.CreatedBy.LastName),
                    _ => query
                };
            }

            return await Task.FromResult(query);
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