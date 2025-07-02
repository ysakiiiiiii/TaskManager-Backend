using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DTOs.TaskAssignment;
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
            var assignments = await _taskAssignmentRepository.GetTaskAssignmentByIdAsync(taskId);
            if (assignments == null) return null;
            return _mapper.Map<TaskAssignmentDto>(assignments);
        }

        public async Task<TaskAssignmentDto> AssignUserToTaskAsync(int taskId, string currentUser, AddTaskAssignmentRequestDto dto)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null) return null;

            if (task.CreatedById != currentUser)
                throw new UnauthorizedAccessException("You are not the owner of this task");

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
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null) return null;

            if (task.CreatedById != currentUser)
                throw new UnauthorizedAccessException("You are not the owner of this task");

            var existing = await _taskAssignmentRepository.GetTaskAssignmentByIdAsync(taskId);
            if (existing != null)
                await _taskAssignmentRepository.RemoveAssignmentAsync(existing);

            var newAssignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = dto.UserId
            };

            var added = await _taskAssignmentRepository.AssignUsersToTaskAsync(newAssignment);
            return _mapper.Map<TaskAssignmentDto>(added);
        }

        public async Task<bool?> RemoveAssignmentsAsync(int taskId, string currentUser)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                return null;

            if (task.CreatedById != currentUser)
                throw new UnauthorizedAccessException("You are not the owner of this task");

            var existingAssignment = await _taskAssignmentRepository.GetTaskAssignmentByIdAsync(taskId);
            if (existingAssignment == null)
                return false;

            await _taskAssignmentRepository.RemoveAssignmentAsync(existingAssignment);

            return true;
        }

    }


}

