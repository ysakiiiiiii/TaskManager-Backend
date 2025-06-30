using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ICheckListRepository
    {
        Task<List<CheckList>> GetCheckListByTaskAsync(int taskId);
        Task<CheckList?> GetCheckListById(int checkListId);
        Task<CheckList> CreateCheckListAsync(int taskId, CheckList checkList);
        Task<CheckList> UpdateCheckListAsync(CheckList checkList);
        Task<bool?> DeleteCheckListAsync(CheckList checkList);
    }
}
