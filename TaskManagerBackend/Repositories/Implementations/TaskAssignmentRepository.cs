using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Repositories.Implementations
{
    public class TaskAssignmentRepository : ITaskAssignmentRepository
    {
        private readonly TaskDbContext _dbContext;

        public TaskAssignmentRepository(TaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskAssignment> GetTaskAssignmentByIdAsync(int taskId)
        {
            return await _dbContext.TaskAssignments
                .AsNoTracking() 
                .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        public async Task<TaskAssignment> AssignUsersToTaskAsync(TaskAssignment assignment)
        {
            var existing = await _dbContext.TaskAssignments
                .FirstOrDefaultAsync(a => a.TaskId == assignment.TaskId && a.UserId == assignment.UserId);

            if (existing != null)
            {
                return existing; 
            }

            await _dbContext.TaskAssignments.AddAsync(assignment);
            await _dbContext.SaveChangesAsync();
            return assignment;
        }

        public async Task<bool> RemoveAssignmentAsync(TaskAssignment assignment)
        {
            var existing = await _dbContext.TaskAssignments
                .FirstOrDefaultAsync(a => a.TaskId == assignment.TaskId && a.UserId == assignment.UserId);

            if (existing != null)
            {
                _dbContext.TaskAssignments.Remove(existing);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }

}
