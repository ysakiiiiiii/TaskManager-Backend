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

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpGet("{taskId}", Name = "GetCommentsByTask")]
        public async Task<IActionResult> GetCommentsByTaskAsync([FromRoute] int taskId)
        {
            var comments = await commentService.GetCommentsByTask(taskId);

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
        public async Task<IActionResult> CreateCommentAsync([FromRoute] int taskId,[FromBody]CreateCommentDto createCommentDto)
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

            var createdComment = await commentService.CreateComment(taskId, createCommentDto, userId);

            if (createdComment == null)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the comment." });
            }


            return CreatedAtAction("GetCommentsByTask", new { taskId = createdComment.TaskId }, createdComment);
        }
    }
}
