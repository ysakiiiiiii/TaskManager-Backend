using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByTask(int taskId);
        Task<Comment> CreateComment(Comment comment);
    }
}
