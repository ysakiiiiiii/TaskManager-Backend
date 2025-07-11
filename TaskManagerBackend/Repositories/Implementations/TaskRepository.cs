using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.CheckList;
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
                .Include(t => t.Comments)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.CreatedBy)
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

            // Role-based filtering for regular users
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

            // Filter by search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    t.AssignedUsers.Any(au =>
                        (au.User.FirstName + " " + au.User.LastName).Contains(search)));
            }

            // Filter by category, priority, and status
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(t => t.Category.Name == category);

            if (!string.IsNullOrWhiteSpace(priority))
                query = query.Where(t => t.Priority.Name == priority);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status.Name == status);

            // Sorting logic
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

        public async Task<bool> UpdateStatusAsync(int taskId, string statusName)
        {
            var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) return false;

            var status = await dbContext.Statuses.FirstOrDefaultAsync(s => s.Name == statusName);
            if (status == null) return false;

            task.StatusId = status.Id;
            task.DateModified = DateTime.UtcNow;

            dbContext.Tasks.Update(task);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task UpdateTaskAssignmentsAsync(int taskId, List<string> assignedUserIds)
        {
            var existingAssignments = await dbContext.TaskAssignments
                .Where(ta => ta.TaskId == taskId)
                .ToListAsync();

            dbContext.TaskAssignments.RemoveRange(existingAssignments);

            var newAssignments = assignedUserIds.Select(uid => new TaskAssignment
            {
                TaskId = taskId,
                UserId = uid
            }).ToList();

            await dbContext.TaskAssignments.AddRangeAsync(newAssignments);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateTaskChecklistItemsAsync(int taskId, List<CheckListDto> checklistItems)
        {
            var existingItems = await dbContext.ChecklistItems
                .Where(cl => cl.TaskId == taskId)
                .ToListAsync();

            dbContext.ChecklistItems.RemoveRange(existingItems);

            var newItems = checklistItems.Select(item => new CheckList
            {
                TaskId = taskId,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            }).ToList();

            await dbContext.ChecklistItems.AddRangeAsync(newItems);
            await dbContext.SaveChangesAsync();
        }

    }
}
