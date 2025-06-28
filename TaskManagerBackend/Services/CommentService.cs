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

        public async Task<CommentDto> CreateCommentAsync(int taskId, CreateCommentRequestDto createCommentDto, string userId)
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

        public async Task<CommentDto?> UpdateCommentAsync(int commentId, UpdateCommentRequestDto updateCommentDto, string userId)
        {
            var comment = await commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null) return null;
            if (comment.UserId != userId) return new CommentDto();

            comment.Content = updateCommentDto.Content;
            comment.DateUpdated = DateTime.UtcNow;

            var result = await commentRepository.UpdateCommentAsync(comment);
            if (result == null) return null;

            return mapper.Map<CommentDto>(result);
        }


        public async Task<bool?> DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await commentRepository.GetCommentByIdAsync(commentId);

            if (comment == null)
                return null;
            if (comment.UserId != userId)
                return false;

            return await commentRepository.DeleteCommentAsync(comment);
        }


        
    }
}
