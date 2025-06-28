using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TaskDbContext dbContext;

        public CommentRepository(TaskDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Comment> CreateComment(Comment comment)
        {
            await dbContext.AddAsync(comment);
            await dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetCommentsByTask(int taskId)
        {
            var comments = await dbContext.Comments
                .Where(c => c.TaskId == taskId)
                .ToListAsync();

            return comments;
        }
    }
}
