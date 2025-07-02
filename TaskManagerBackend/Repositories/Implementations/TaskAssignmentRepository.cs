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
            return await _dbContext.TaskAssignments.FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        public async Task<TaskAssignment> AssignUsersToTaskAsync(TaskAssignment assignments)
        {
            await _dbContext.TaskAssignments.AddAsync(assignments);
            await _dbContext.SaveChangesAsync();
            return assignments;
        }

        public async Task<bool> RemoveAssignmentAsync(TaskAssignment assignment)
        {
            _dbContext.TaskAssignments.Remove(assignment);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }

}
