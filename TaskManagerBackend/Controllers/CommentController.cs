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
            return Ok(comments);
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> CreateCommentAsync([FromRoute] int taskId, [FromBody] CreateCommentRequestDto createCommentDto)
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

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int commentId, [FromBody] UpdateCommentRequestDto updateCommentDto)
        {   
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized(new { message = "Account not authorized" });
            }

            var updatedComment = await commentService.UpdateCommentAsync(commentId, updateCommentDto, userId);

            if (updatedComment == null)
               return NotFound(new { Message = "Comment not found or unauthorized" });

            if(updatedComment.Id == 0)
                return StatusCode(403, new { Message = "You are not authorized to update this comment." });

            return Ok(updatedComment);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute]int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
            {
                return Unauthorized(new { message = "Account not authorized" });
            }
            var result = await commentService.DeleteCommentAsync(commentId, userId);

            if (result == null)
                return NotFound(new { Message = "Comment not found." });
            
            if(result == false)
                return StatusCode(403, new { Message = "You are not authorized to update this comment." });

            return Ok("Successfully delete the comment");

        }
    }
}
