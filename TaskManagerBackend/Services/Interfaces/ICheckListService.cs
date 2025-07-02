using TaskManagerBackend.DTOs.CheckList;

namespace TaskManagerBackend.Services.Interfaces
{
    public interface ICheckListService
    {
        Task<List<CheckListDto?>> GetCheckListByTaskAsync(int taskId);
        Task<List<CheckListDto>> CreateCheckListAsync(int taskId, AddCheckListItemDto addCheckListItemDto);
        Task<List<CheckListDto?>> UpdateCheckListAsync(UpdateCheckListDto updateCheckListDto);
        Task<bool?> DeleteCheckListAsync(int checkListId);
    }
}
