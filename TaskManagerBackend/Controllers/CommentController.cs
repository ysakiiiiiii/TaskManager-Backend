using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet("{taskId}", Name = "GetCommentsByTask")]
        public async Task<IActionResult> GetCommentsByTaskAsync([FromRoute] int taskId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByTaskAsync(taskId);
                return Ok(ApiResponse.SuccessResponse(comments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting comments for task ID {taskId}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving comments"));
            }
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> CreateCommentAsync([FromRoute] int taskId, [FromBody] CreateCommentRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                }

                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
                }

                if (string.IsNullOrWhiteSpace(dto.Content))
                {
                    return BadRequest(ApiResponse.ErrorResponse("Comment content cannot be empty"));
                }

                var createdComment = await _commentService.CreateCommentAsync(taskId, dto, userId);

                if (createdComment == null)
                {
                    return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while creating the comment"));
                }

                return CreatedAtAction("GetCommentsByTask",
                    new { taskId = createdComment.TaskId },
                    ApiResponse.SuccessResponse(createdComment, "Comment created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating comment for task ID {taskId}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while creating the comment"));
            }
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int commentId, [FromBody] UpdateCommentRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                }

                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
                }

                var updatedComment = await _commentService.UpdateCommentAsync(commentId, dto, userId);

                if (updatedComment == null)
                    return NotFound(ApiResponse.ErrorResponse("Comment not found"));

                if (updatedComment.Id == 0)
                    return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to update this comment"));

                return Ok(ApiResponse.SuccessResponse(updatedComment, "Comment updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating comment with ID {commentId}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while updating the comment"));
            }
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int commentId)
        {
            try
            {
                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
                }

                var result = await _commentService.DeleteCommentAsync(commentId, userId);

                if (result == null)
                    return NotFound(ApiResponse.ErrorResponse("Comment not found"));

                if (result == false)
                    return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to delete this comment"));

                return Ok(ApiResponse.SuccessResponse(null, "Comment deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting comment with ID {commentId}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the comment"));
            }
        }
    }
}
