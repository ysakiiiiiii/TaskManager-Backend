using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Services
{
    public interface ICommentService
    {
        Task <List<CommentDto>> GetCommentsByTaskAsync(int taskId);
        Task<CommentDto> GetCommentByIdAsync(int commentId);
        Task<CommentDto> CreateCommentAsync(int taskId, CreateCommentDto createCommentDto, string userId);
        Task<bool> DeleteCommentAsync(int commentId);
    }
}
