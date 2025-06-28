using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Services;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        private readonly ITaskService taskService;

        public CommentController(ICommentService commentService, ITaskService taskService)
        {
            this.commentService = commentService;
            this.taskService = taskService;
        }

        [HttpGet("{taskId}", Name = "GetCommentsByTask")]
        public async Task<IActionResult> GetCommentsByTaskAsync([FromRoute] int taskId)
        {
            var comments = await commentService.GetCommentsByTaskAsync(taskId);

            if (comments == null || !comments.Any())
            {
                return Ok(new
                {
                    Message = "No comments found for this task.",
                    comments = new List<CommentDto>()
                });
            }
            return Ok(comments);
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> CreateCommentAsync([FromRoute] int taskId, [FromBody] CreateCommentDto createCommentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            if (createCommentDto == null || string.IsNullOrWhiteSpace(createCommentDto.Content))
            {
                return BadRequest(new { Message = "Invalid comment data." });
            }

            var createdComment = await commentService.CreateCommentAsync(taskId, createCommentDto, userId);

            if (createdComment == null)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the comment." });
            }


            return CreatedAtAction("GetCommentsByTask", new { taskId = createdComment.TaskId }, createdComment);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute]int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
            {
                return Unauthorized(new { message = "Account not authorized" });
            }

            var comment = await commentService.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                return NotFound(new { Message = "Comment not found." });
            }

            var task = await taskService.GetTaskByIdAsync(comment.TaskId);

            if (task == null)
            {
                return NotFound(new { Message = "Task not found." });
            }

            if(task.CreatedById != userId && comment.UserId != userId)
            {
                return StatusCode(403, new { Message = "You are not authorized to delete this comment." });
            }

            var deleteComment = await commentService.DeleteCommentAsync(commentId);

            if (!deleteComment)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the comment." });
            }

            return Ok("Successfully delete the comment");

        }
    }
}
