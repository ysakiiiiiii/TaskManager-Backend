using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TaskDbContext dbContext;

        public CommentRepository(TaskDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            return comment;

        }

        public async Task<List<Comment>> GetCommentsByTaskAsync(int taskId)
        {
            return await dbContext.Comments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.User)
                .OrderBy(c => c.DateCreated)
                .ToListAsync();
        }


        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await dbContext.AddAsync(comment);
            await dbContext.SaveChangesAsync();
            return comment;
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
