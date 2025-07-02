using TaskManagerBackend.DTOs.TaskAssignment;

namespace TaskManagerBackend.Services.Interfaces
{
    public interface ITaskAssignmentService
    {
        Task<TaskAssignmentDto>GetTaskAssignmentByIdAsync(int taskId);
        Task<TaskAssignmentDto> AssignUserToTaskAsync(int taskId, string currentUser, AddTaskAssignmentRequestDto dto);
        Task<TaskAssignmentDto> UpdateAssignedUsersAsync(int taskId, string currentUser, UpdateTaskAssignmentRequestDto dto);
        Task<bool?> RemoveAssignmentsAsync(int taskId, string currentUser);
    }
}
