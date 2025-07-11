using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Exceptions;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ITaskAssignmentRepository _taskAssignmentRepository;

    public TaskService(
        ITaskRepository taskRepository,
        IMapper mapper,
        ITaskAssignmentRepository taskAssignmentRepository)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _taskAssignmentRepository = taskAssignmentRepository;
    }

    public async Task<PaginatedTaskResponse> GetAllTasksAsync(TaskQueryParameters parameters, string? userId, string? userRole)
    {
        var query = await _taskRepository.GetFilteredTasksQueryAsync(
            parameters.Search,
            parameters.Category,
            parameters.Priority,
            parameters.Status,
            null,
            true,
            userId,
            userRole,
            parameters.Type);

        var totalCount = await query.CountAsync();

        if (parameters.PageSize == 0)
            parameters.PageSize = totalCount == 0 ? 1 : totalCount;

        var pagedTasks = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var taskDtos = _mapper.Map<List<TaskDto>>(pagedTasks);

        return new PaginatedTaskResponse
        {
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize),
            Items = taskDtos
        };
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
            throw new ForbiddenException("User not authenticated");

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

        return await GetTaskByIdAsync(createdTask.Id);
    }

    public async Task<TaskDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto dto, string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ForbiddenException("User not authenticated");

        var task = await _taskRepository.GetTaskByIdAsync(taskId)
            ?? throw new NotFoundException($"Task with ID {taskId} not found");

        if (task.CreatedById != userId)
            throw new ForbiddenException("You are not authorized to update this task");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.CategoryId = dto.CategoryId;
        task.PriorityId = dto.PriorityId;
        task.StatusId = dto.StatusId;
        task.DueDate = dto.DueDate;
        task.DateModified = DateTime.UtcNow;

        if (dto.AssignedUserIds != null)
        {
            await _taskRepository.UpdateTaskAssignmentsAsync(taskId, dto.AssignedUserIds);
        }

        if (dto.ChecklistItems != null)
        {
            await _taskRepository.UpdateTaskChecklistItemsAsync(taskId, dto.ChecklistItems);
        }

        await _taskRepository.UpdateTaskAsync(task);

        return await GetTaskByIdAsync(taskId);
    }

    public async Task<bool> UpdateStatusAsync(int taskId, string statusName)
    {
        return await _taskRepository.UpdateStatusAsync(taskId, statusName);
    }

    public async Task DeleteTaskAsync(int taskId, string userId, bool isAdmin)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ForbiddenException("User not authenticated");

        var task = await _taskRepository.GetTaskByIdAsync(taskId)
            ?? throw new NotFoundException($"Task with ID {taskId} not found");

        if (task.CreatedById != userId && !isAdmin)
            throw new ForbiddenException("You are not authorized to delete this task");

        await _taskRepository.DeleteTaskAsync(task);
    }


}
