using TaskManagerBackend.DTOs.CheckList;

namespace TaskManagerBackend.Services
{
    public interface ICheckListService
    {
        Task<List<CheckListDto>> GetCheckListByTaskAsync(int taskId);
        Task<CheckListDto> CreateCheckListAsync(int taskId, AddCheckListItemDto addCheckListItemDto);

    }
}
