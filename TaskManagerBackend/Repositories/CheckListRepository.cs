using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public class CheckListRepository : ICheckListRepository
    {
        private readonly TaskDbContext dbContext;
        public CheckListRepository(TaskDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TaskDbContext DbContext { get; }

        public async Task<CheckList> CreateCheckListAsync(int taskId, CheckList checkList)
        {
            await dbContext.ChecklistItems.AddAsync(checkList);
            await dbContext.SaveChangesAsync();

            return checkList;
        }

        public async Task<List<CheckList>> GetCheckListByTaskAsync(int taskId)
        {
            var checkList = await dbContext.ChecklistItems
                .Where(c => c.TaskId == taskId)
                .ToListAsync();

            
            return checkList;
        }
    }
}
