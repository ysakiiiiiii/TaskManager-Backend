using TaskManagerBackend.Models.Domain;

public interface ITaskAssignmentRepository
{
    Task<TaskAssignment> GetTaskAssignmentByIdAsync(int taskId);
    Task<TaskAssignment> AssignUsersToTaskAsync(TaskAssignment assignments);
    Task<bool> RemoveAssignmentAsync(TaskAssignment assignment);
}
