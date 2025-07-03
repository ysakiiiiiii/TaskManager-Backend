using AutoMapper;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Exceptions;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Implementations;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, ITaskRepository taskRepository ,IMapper mapper)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<CommentDto> GetCommentByIdAsync(int commentId)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId)
                ?? throw new NotFoundException($"Comment with ID {commentId} not found");

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<List<CommentDto>> GetCommentsByTaskAsync(int taskId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId)
                ?? throw new NotFoundException($"Task with ID {taskId} not found");

            var comments = await _commentRepository.GetCommentsByTaskAsync(taskId);

            if (comments == null || !comments.Any())
                throw new NotFoundException($"No comment found in task with ID {taskId}");

            return _mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<CommentDto> CreateCommentAsync(int taskId, CreateCommentRequestDto dto, string userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (!task.AssignedUsers.Any(u => u.UserId == userId))
                throw new ForbiddenException("You are not assigned to this task and cannot comment.");

            if(task == null) throw new NotFoundException($"Task with ID {taskId} not found");



            var comment = _mapper.Map<Comment>(dto);
            comment.TaskId = taskId;
            comment.UserId = userId;
            comment.DateCreated = DateTime.UtcNow;

            var created = await _commentRepository.CreateCommentAsync(comment);
            return _mapper.Map<CommentDto>(created);
        }

        public async Task<CommentDto> UpdateCommentAsync(int commentId, UpdateCommentRequestDto dto, string userId)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            
            if (comment.UserId != userId)
                throw new ForbiddenException("You are not authorized to update this comment");

            if(comment == null) throw new NotFoundException($"Comment with ID {commentId} not found");

            _mapper.Map(dto, comment);

            var updated = await _commentRepository.UpdateCommentAsync(comment);
            return _mapper.Map<CommentDto>(updated);
        }

        public async Task DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId)
                ?? throw new NotFoundException($"Comment with ID {commentId} not found");

            if (comment.UserId != userId)
                throw new ForbiddenException("You are not authorized to delete this comment");

            var success = await _commentRepository.DeleteCommentAsync(comment);
            if (!success)
                throw new ApplicationException("Failed to delete comment");
        }
    }
}
