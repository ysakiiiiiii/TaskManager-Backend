using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;

namespace TaskManagerBackend.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TaskDbContext dbContext;

        public CommentRepository(TaskDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await dbContext.AddAsync(comment);
            await dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            return comment;

        }

        public async Task<List<Comment>> GetCommentsByTaskAsync(int taskId)
        {
            var comments = await dbContext.Comments
                .Where(c => c.TaskId == taskId)
                .ToListAsync();

            return comments;
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            dbContext.Comments.Update(comment);
            await dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(Comment comment)
        {
            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();
            return true;
        }


    }
}
