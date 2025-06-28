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

        public async Task<CommentDto> CreateComment(int taskId, CreateCommentDto createCommentDto, string userId)
        {
            var comments = new Comment
            {
                TaskId = taskId,
                UserId = userId,
                Content = createCommentDto.Content,
                DateCreated = DateTime.UtcNow,
            };

            var createdComment = await commentRepository.CreateComment(comments);
            return mapper.Map<CommentDto>(createdComment);
        }

        public async Task<List<CommentDto>> GetCommentsByTask(int taskId)
        {
            var comments = await commentRepository.GetCommentsByTask(taskId);
            return mapper.Map<List<CommentDto>>(comments);

        }
    }
}
