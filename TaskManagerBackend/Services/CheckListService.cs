using AutoMapper;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Services
{
    public class CheckListService : ICheckListService
    {
        private readonly ICheckListRepository checkListRepository;
        private readonly IMapper mapper;

        public CheckListService(ICheckListRepository checkListRepository, IMapper mapper)
        {
            this.checkListRepository = checkListRepository;
            this.mapper = mapper;
        }

        public async Task<CheckListDto> CreateCheckListAsync(int taskId, AddCheckListItemDto addCheckListItemDto)
        {
            var checkListItem = new CheckList
            {
                TaskId = taskId,
                Description = addCheckListItemDto.Description,
                IsCompleted = false
            };

            var createdCheckListItem = await checkListRepository.CreateCheckListAsync(taskId, checkListItem);

            return mapper.Map<CheckListDto>(createdCheckListItem);
        }

        public async Task<List<CheckListDto>> GetCheckListByTaskAsync(int taskId)
        {
            var checkList = await checkListRepository.GetCheckListByTaskAsync(taskId);

            return mapper.Map<List<CheckListDto>>(checkList);

        }

    }
}
