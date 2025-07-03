using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Repositories.Implementations
{
    public class CheckListRepository : ICheckListRepository
    {
        private readonly TaskDbContext _dbContext;
        public CheckListRepository(TaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CheckList>> CreateCheckListAsync(List<CheckList> checkLists)
        {
            await _dbContext.ChecklistItems.AddRangeAsync(checkLists);
            await _dbContext.SaveChangesAsync();
            return checkLists;
        }

        public async Task<bool> DeleteCheckListAsync(CheckList checkList)
        {
            _dbContext.ChecklistItems.Remove(checkList);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CheckList?> GetCheckListById(int checkListId) => await _dbContext.ChecklistItems.FirstOrDefaultAsync(c => c.Id == checkListId);

        public async Task<List<CheckList>> GetCheckListByTaskAsync(int taskId)
        {
            var checkList = await _dbContext.ChecklistItems
                .Where(c => c.TaskId == taskId)
                .ToListAsync();

            return checkList;
        }

        public async Task<CheckList> UpdateCheckListAsync(CheckList checkList)
        {
            _dbContext.Update(checkList);
            await _dbContext.SaveChangesAsync();
            return checkList;
        }
    }
}
