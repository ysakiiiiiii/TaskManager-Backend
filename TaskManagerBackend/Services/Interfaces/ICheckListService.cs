using TaskManagerBackend.DTOs.CheckList;

namespace TaskManagerBackend.Services.Interfaces
{
    public interface ICheckListService
    {
        Task<List<CheckListDto?>> GetCheckListByTaskAsync(int taskId);
        Task<List<CheckListDto>> CreateCheckListAsync(int taskId, string userId, AddCheckListItemDto dto);
        Task<List<CheckListDto>> UpdateCheckListAsync(string userId, UpdateCheckListRequestDto dto);
        Task DeleteCheckListAsync(string userId, int id);

        Task ToggleIsCompletedAsync(string userId, int checkListId);
    }
}
