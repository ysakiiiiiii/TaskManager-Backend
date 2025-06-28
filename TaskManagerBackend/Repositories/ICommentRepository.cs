using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByTaskAsync(int taskId);
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment> DeleteCommentAsync(int commentId);
    }
}
