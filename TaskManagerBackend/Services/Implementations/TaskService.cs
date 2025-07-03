// Services/TaskService.cs
using AutoMapper;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Exceptions;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;

        public TaskService(ITaskRepository taskRepository, IMapper mapper, ITaskAssignmentRepository taskAssignmentRepository)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _taskAssignmentRepository = taskAssignmentRepository;
        }

        public async Task<List<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            if (tasks == null || !tasks.Any())
            {
                throw new NotFoundException("No tasks found");
            }
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id)
                ?? throw new NotFoundException($"Task with ID {id} not found");
            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateTaskAsync(AddTaskRequestDto dto, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ForbiddenException("User not authenticated");
            }

            var taskItem = _mapper.Map<TaskItem>(dto);

            taskItem.CreatedById = userId;
            taskItem.DateCreated = DateTime.UtcNow;

            var createdTask = await _taskRepository.CreateTaskAsync(taskItem);

            if (dto.AssignedUserIds != null && dto.AssignedUserIds.Any())
            {
                foreach (var assignedUserId in dto.AssignedUserIds)
                {
                    var assignment = new TaskAssignment
                    {
                        TaskId = createdTask.Id,
                        UserId = assignedUserId
                    };
                    await _taskAssignmentRepository.AssignUsersToTaskAsync(assignment);
                }
            }

            return _mapper.Map<TaskDto>(createdTask);
        }

        public async Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto dto, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ForbiddenException("User not authenticated");
            }

            var task = await _taskRepository.GetTaskByIdAsync(taskId)
                ?? throw new NotFoundException($"Task with ID {taskId} not found");

            if (task.CreatedById != userId)
                throw new ForbiddenException("You are not authorized to update this task");

            _mapper.Map(dto, task);

            if (dto.AssignedUserIds != null)
            {
                task.AssignedUsers = dto.AssignedUserIds
                    .Select(uid => new TaskAssignment { UserId = uid, TaskId = taskId })
                    .ToList();
            }

            if (dto.ChecklistItems != null)
            {
                foreach (var item in dto.ChecklistItems)
                {
                    if (!task.CheckListItems.Any(existing => string.Equals(existing.Description.Trim(), item.Description.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        task.CheckListItems.Add(new CheckList
                        {
                            Description = item.Description,
                            IsCompleted = false,
                            TaskId = taskId
                        });
                    }
                }
            }

            var updatedTask = await _taskRepository.UpdateTaskAsync(task);
            return _mapper.Map<TaskDto>(updatedTask);
        }

        public async Task DeleteTaskAsync(int taskId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ForbiddenException("User not authenticated");
            }

            var task = await _taskRepository.GetTaskByIdAsync(taskId)
                ?? throw new NotFoundException($"Task with ID {taskId} not found");

            if (task.CreatedById != userId)
                throw new ForbiddenException("You are not authorized to delete this task");

            await _taskRepository.DeleteTaskAsync(task);
        }
    }
}