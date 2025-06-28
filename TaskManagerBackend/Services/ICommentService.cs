using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Services
{
    public interface ICommentService
    {
        Task <List<CommentDto>> GetCommentsByTask(int taskId);
        Task<CommentDto> CreateComment(int taskId, CreateCommentDto createCommentDto, string userId);
    }
}
