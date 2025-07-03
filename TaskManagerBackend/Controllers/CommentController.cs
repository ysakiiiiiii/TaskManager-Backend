using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{taskId}", Name = "GetCommentsByTask")]
        public async Task<IActionResult> GetCommentsByTaskAsync([FromRoute] int taskId)
        {
            var comments = await _commentService.GetCommentsByTaskAsync(taskId);
            return Ok(ApiResponse.SuccessResponse(comments));
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> CreateCommentAsync([FromRoute] int taskId, [FromBody] CreateCommentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.ToErrorList()));

            var userId = this.GetUserId();
            var createdComment = await _commentService.CreateCommentAsync(taskId, dto, userId);

            return CreatedAtAction(nameof(GetCommentsByTaskAsync),
                new { taskId = createdComment.TaskId },
                ApiResponse.SuccessResponse(createdComment, "Comment created successfully"));
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int commentId, [FromBody] UpdateCommentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.ToErrorList()));

            var userId = this.GetUserId();
            var updatedComment = await _commentService.UpdateCommentAsync(commentId, dto, userId);

            return Ok(ApiResponse.SuccessResponse(updatedComment, "Comment updated successfully"));
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int commentId)
        {
            var userId = this.GetUserId();
            await _commentService.DeleteCommentAsync(commentId, userId);
            return Ok(ApiResponse.SuccessResponse(null, "Comment deleted successfully"));
        }
    }
}
