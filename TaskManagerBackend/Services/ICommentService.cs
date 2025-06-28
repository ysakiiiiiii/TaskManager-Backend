using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Services
{
    public interface ICommentService
    {
        Task <List<CommentDto>> GetCommentsByTaskAsync(int taskId);
        Task<CommentDto> GetCommentByIdAsync(int commentId);
        Task<CommentDto> CreateCommentAsync(int taskId, CreateCommentRequestDto createCommentDto, string userId);
        Task<CommentDto?> UpdateCommentAsync(int commentId, UpdateCommentRequestDto updateCommentDto, string userId);
        Task<bool?> DeleteCommentAsync(int commentId, string userId);
    }
}
