using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ICheckListRepository
    {
        Task<List<CheckList>> GetCheckListByTaskAsync(int taskId);
        Task<CheckList> CreateCheckListAsync(int taskId, CheckList checkList);

    }
}
