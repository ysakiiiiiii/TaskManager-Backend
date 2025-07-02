using AutoMapper;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;   
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<List<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            return task == null ? null : _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateTaskAsync(AddTaskRequestDto dto, string userId)
        {
            var taskItem = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                PriorityId = dto.PriorityId,
                StatusId = dto.StatusId,
                DueDate = dto.DueDate,
                CreatedById = userId,
                DateCreated = DateTime.UtcNow,
                AssignedUsers = dto.AssignedUserIds.Select(uid => new TaskAssignment { UserId = uid }).ToList(),
                CheckListItems = dto.ChecklistItems.Select(c => new CheckList { Description = c.Description, IsCompleted = false }).ToList(),
            };

            var createdTask = await _taskRepository.CreateTaskAsync(taskItem);
            return _mapper.Map<TaskDto>(createdTask);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskRequestDto dto, string userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null) return null;
            if (task.CreatedById != userId) return new TaskDto();

            // Update main task properties
            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.CategoryId != null) task.CategoryId = dto.CategoryId.Value;
            if (dto.PriorityId != null) task.PriorityId = dto.PriorityId.Value;
            if (dto.StatusId != null) task.StatusId = dto.StatusId.Value;
            if (dto.DueDate != null) task.DueDate = dto.DueDate;

            // Update assigned users if provided
            if (dto.AssignedUserIds != null)
            {
                task.AssignedUsers = dto.AssignedUserIds
                    .Select(uid => new TaskAssignment { UserId = uid, TaskId = taskId })
                    .ToList();
            }

            // Update checklist items if provided
            if (dto.ChecklistItems != null)
            {
                foreach (var item in dto.ChecklistItems)
                {
                    if (!task.CheckListItems.Any(existing =>
                        string.Equals(existing.Description.Trim(), item.Description.Trim(),
                        StringComparison.OrdinalIgnoreCase)))
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

        public async Task<bool?> DeleteTaskAsync(int taskId, string userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null) return null;
            if (task.CreatedById != userId) return false;

            return await _taskRepository.DeleteTaskAsync(task);
        }
    }
}