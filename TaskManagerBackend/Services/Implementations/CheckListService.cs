using System.Threading.Tasks;
using AutoMapper;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Implementations;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Services
{
    public class CheckListService : ICheckListService
    {
        private readonly ICheckListRepository checkListRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper mapper;

        public CheckListService(ICheckListRepository checkListRepository,ITaskRepository taskRepository, IMapper mapper)
        {
            this.checkListRepository = checkListRepository;
            _taskRepository = taskRepository;
            this.mapper = mapper;
        }

        public async Task<List<CheckListDto>> CreateCheckListAsync(int taskId, AddCheckListItemDto dto)
        {
            //var task = _taskRepository.GetTaskByIdAsync(taskId);
            //if (task == null)
            //{
            //    throw new NotFound(ApiResponse.ErrorResponse("Y"));
            //}

            var checkListItems = dto.Items.Select(item => new CheckList
            {
                TaskId = taskId,
                Description = item.Description,
                IsCompleted = false
            }).ToList();

            var createdItems = await checkListRepository.CreateCheckListAsync(checkListItems);

            return mapper.Map<List<CheckListDto>>(createdItems);
        }


        public async Task<bool?> DeleteCheckListAsync(int checkListId)
        {
            var checkListItem = await checkListRepository.GetCheckListById(checkListId);
            if (checkListItem == null)
            {
                return null;
            }

            return await checkListRepository.DeleteCheckListAsync(checkListItem);
        }

        public async Task<List<CheckListDto?>> GetCheckListByTaskAsync(int taskId)
        {
            var checkList = await checkListRepository.GetCheckListByTaskAsync(taskId);

            return mapper.Map<List<CheckListDto>>(checkList);

        }

        public async Task<List<CheckListDto?>> UpdateCheckListAsync(UpdateCheckListDto updateCheckListDto)
        {
            var updatedCheckLists = new List<CheckListDto>();

            foreach (var item in updateCheckListDto.Items)
            {
                var existingItem = await checkListRepository.GetCheckListById(item.Id);

                if (existingItem != null)
                {
                    existingItem.Description = item.Description;

                    var updatedItem = await checkListRepository.UpdateCheckListAsync(existingItem);
                    updatedCheckLists.Add(mapper.Map<CheckListDto>(updatedItem));
                }
            }

            return updatedCheckLists;
        }

    }
}
