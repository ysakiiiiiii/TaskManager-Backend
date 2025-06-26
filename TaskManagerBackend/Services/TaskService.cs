using AutoMapper;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

public class TaskService : ITaskService
{
    private readonly ITaskRepository taskRepository;
    private readonly IMapper mapper;

    public TaskService(ITaskRepository taskRepository, IMapper mapper)
    {
        this.taskRepository = taskRepository;
        this.mapper = mapper;
    }

    public async Task<List<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await taskRepository.GetAllTasksAsync();
        return mapper.Map<List<TaskDto>>(tasks);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        var task = await taskRepository.GetTaskByIdAsync(id);
        return task == null ? null : mapper.Map<TaskDto>(task);
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
            AssignedUsers = dto.AssignedUsersId.Select(uid => new TaskAssignment { UserId = uid }).ToList(),
            CheckListItems = dto.ChecklistItems.Select(c => new CheckList { Description = c.Description, IsCompleted = false }).ToList(),
            Attachments = dto.Attachments?.Select(a => new Attachment { FileName = a.FileName, FilePath = a.FilePath, UploadedById = userId }).ToList()
        };

        var createdTask = await taskRepository.CreateTaskAsync(taskItem);
        return mapper.Map<TaskDto>(createdTask);
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskRequestDto dto, string userId)
    {
        var task = await taskRepository.GetTaskByIdAsync(id);
        if (task == null) return null;

        task.Title = dto.Title ?? task.Title;
        task.Description = dto.Description ?? task.Description;
        task.CategoryId = dto.CategoryId ?? task.CategoryId;
        task.PriorityId = dto.PriorityId ?? task.PriorityId;
        task.StatusId = dto.StatusId ?? task.StatusId;
        task.DueDate = dto.DueDate ?? task.DueDate;

        if (dto.AssignedUserIds != null)
        {
            task.AssignedUsers = dto.AssignedUserIds.Select(uid => new TaskAssignment { UserId = uid, TaskId = id }).ToList();
        }

        if (dto.ChecklistItems != null)
        {
            task.CheckListItems = dto.ChecklistItems.Select(c => new CheckList { Description = c.Description, IsCompleted = c.IsCompleted ?? false, TaskId = id }).ToList();
        }

        var updatedTask = await taskRepository.UpdateTaskAsync(task);
        return mapper.Map<TaskDto>(updatedTask);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var result = await taskRepository.DeleteTaskAsync(id);
        return result != null;
    }
}
