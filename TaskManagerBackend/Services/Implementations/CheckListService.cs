using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.Exceptions;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Implementations;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Services
{
    public class CheckListService : ICheckListService
    {
        private readonly ICheckListRepository _checkListRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public CheckListService(ICheckListRepository checkListRepository, ITaskRepository taskRepository, IMapper mapper)
        {
            _checkListRepository = checkListRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<List<CheckListDto>> GetCheckListByTaskAsync(int taskId)
        {
            var list = await _checkListRepository.GetCheckListByTaskAsync(taskId);
                
            if(list == null || !list.Any())
            {
                throw new NotFoundException("No checklist is found");
            }

            return _mapper.Map<List<CheckListDto>>(list);
        }

        public async Task<List<CheckListDto>> CreateCheckListAsync(int taskId, string userId, AddCheckListItemDto dto)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId) ?? throw new NotFoundException("Task not found");

            if (task.CreatedById != userId && !task.AssignedUsers.Any(u => u.UserId == userId))
                throw new ForbiddenException("You are not allowed to add checklist items to this task");
            
            var items = _mapper.Map<List<CheckList>>(dto.Items);

            foreach (var item in items)
            {
                item.TaskId = taskId;
            }


            var created = await _checkListRepository.CreateCheckListAsync(items);
            return _mapper.Map<List<CheckListDto>>(created);
        }

        public async Task<List<CheckListDto>> UpdateCheckListAsync(string userId, UpdateCheckListRequestDto dto)
        {
            var result = new List<CheckListDto>();

            foreach (var itemDto in dto.Items)
            {
                var existing = await _checkListRepository.GetCheckListById(itemDto.Id) ?? throw new NotFoundException($"Checklist item with ID {itemDto.Id} not found");

                var task = await _taskRepository.GetTaskByIdAsync(existing.TaskId) ?? throw new NotFoundException("Task not found");

                if (!task.AssignedUsers.Any(u => u.UserId == userId))
                    throw new ForbiddenException("You are not allowed to update this checklist item");

                _mapper.Map(itemDto, existing);
                var updated = await _checkListRepository.UpdateCheckListAsync(existing);
                result.Add(_mapper.Map<CheckListDto>(updated));
            }

            return result;
        }

        public async Task DeleteCheckListAsync(string userId, int id)
        {
            var existing = await _checkListRepository.GetCheckListById(id)
                ?? throw new NotFoundException("Checklist item not found");

            var task = await _taskRepository.GetTaskByIdAsync(existing.TaskId)
                ?? throw new NotFoundException("Associated task not found");

            if (!task.AssignedUsers.Any(u => u.UserId == userId))
                throw new ForbiddenException("You are not allowed to delete this checklist item");

            var success = await _checkListRepository.DeleteCheckListAsync(existing);
            if (!success)
                throw new InvalidOperationException("Failed to delete checklist item");
        }

        public async Task ToggleIsCompletedAsync(string userId, int checkListId)
        {
            var item = await _checkListRepository.GetCheckListById(checkListId)
                       ?? throw new NotFoundException("Checklist item not found");

            var task = await _taskRepository.GetTaskByIdAsync(item.TaskId)
                       ?? throw new NotFoundException("Task not found");

            if (task.CreatedById != userId && !task.AssignedUsers.Any(u => u.UserId == userId))
                throw new ForbiddenException("You are not allowed to modify this checklist item");


            item.IsCompleted = !item.IsCompleted;
            await _checkListRepository.UpdateCheckListAsync(item);
        }

    }

}
