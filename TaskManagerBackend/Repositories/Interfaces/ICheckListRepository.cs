using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface ICheckListRepository
    {
        Task<List<CheckList>> GetCheckListByTaskAsync(int taskId);
        Task<CheckList?> GetCheckListById(int checkListId);
        Task<List<CheckList>> CreateCheckListAsync(List<CheckList> checkLists);
        Task<CheckList> UpdateCheckListAsync(CheckList checkList);
        Task<bool?> DeleteCheckListAsync(CheckList checkList);
    }
}
