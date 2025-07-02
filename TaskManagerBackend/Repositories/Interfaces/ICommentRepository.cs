using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByTaskAsync(int taskId);
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Comment comment);
    }
}
