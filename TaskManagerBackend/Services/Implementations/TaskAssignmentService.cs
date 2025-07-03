using AutoMapper;
using TaskManagerBackend.DTOs.TaskAssignment;
using TaskManagerBackend.Exceptions;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskAssignmentService(
            ITaskAssignmentRepository taskAssignmentRepository,
            ITaskRepository taskRepository,
            IMapper mapper)
        {
            _taskAssignmentRepository = taskAssignmentRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskAssignmentDto> GetTaskAssignmentByIdAsync(int taskId)
        {
            var assignments = await _taskAssignmentRepository.GetTaskAssignmentByIdAsync(taskId)
                ?? throw new NotFoundException($"Task assignment with task ID {taskId} not found");

            return _mapper.Map<TaskAssignmentDto>(assignments);
        }

        public async Task<TaskAssignmentDto> AssignUserToTaskAsync(int taskId, string currentUser, AddTaskAssignmentRequestDto dto)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId)
                ?? throw new NotFoundException($"Task with ID {taskId} not found");

            if (task.CreatedById != currentUser)
                throw new ForbiddenException("You are not allowed to assign a user to this task.");

            var assignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = dto.UserId
            };

            var created = await _taskAssignmentRepository.AssignUsersToTaskAsync(assignment);
            return _mapper.Map<TaskAssignmentDto>(created);
        }

        public async Task<TaskAssignmentDto> UpdateAssignedUsersAsync(int taskId, string currentUser, UpdateTaskAssignmentRequestDto dto)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId)
                ?? throw new NotFoundException($"Task with ID {taskId} not found");

            if (task.CreatedById != currentUser)
                throw new ForbiddenException("You are not allowed to update assignments for this task.");

            var existing = await _taskAssignmentRepository.GetTaskAssignmentByIdAsync(taskId);

            if (existing != null)
            {
                await _taskAssignmentRepository.RemoveAssignmentAsync(existing);
            }

            var newAssignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = dto.UserId
            };

            var added = await _taskAssignmentRepository.AssignUsersToTaskAsync(newAssignment);
            return _mapper.Map<TaskAssignmentDto>(added);
        }

        public async Task<bool> RemoveAssignmentsAsync(int taskId, string currentUser)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId)
                ?? throw new NotFoundException($"Task with ID {taskId} not found");

            if (task.CreatedById != currentUser)
                throw new ForbiddenException("You are not allowed to remove assignments for this task.");

            var existingAssignment = await _taskAssignmentRepository.GetTaskAssignmentByIdAsync(taskId)
                ?? throw new NotFoundException($"No assignment found for task ID {taskId}.");

            await _taskAssignmentRepository.RemoveAssignmentAsync(existingAssignment);

            return true;
        }
    }
}
