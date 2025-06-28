using AutoMapper;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IMapper mapper;

        public CommentService(ICommentRepository commentRepository, IMapper mapper)
        {
            this.commentRepository = commentRepository;
            this.mapper = mapper;
        }

        public async Task<CommentDto> CreateCommentAsync(int taskId, CreateCommentDto createCommentDto, string userId)
        {
            var comments = new Comment
            {
                TaskId = taskId,
                UserId = userId,
                Content = createCommentDto.Content,
                DateCreated = DateTime.UtcNow,
            };

            var createdComment = await commentRepository.CreateCommentAsync(comments);
            return mapper.Map<CommentDto>(createdComment);
        }

        public async Task<CommentDto> GetCommentByIdAsync(int commentId)
        {
           var comment = await commentRepository.GetCommentByIdAsync(commentId);
           return mapper.Map<CommentDto>(comment);
        }

        public async Task<List<CommentDto>> GetCommentsByTaskAsync(int taskId)
        {
            var comments = await commentRepository.GetCommentsByTaskAsync(taskId);
            return mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var result = await commentRepository.DeleteCommentAsync(commentId);
            return result != null;
        }

    }
}
