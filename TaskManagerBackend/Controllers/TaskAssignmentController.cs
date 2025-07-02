using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.TaskAssignment;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskAssignmentController : ControllerBase
    {
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly ILogger<TaskAssignmentController> _logger;

        public TaskAssignmentController(ITaskAssignmentService taskAssignmentService, ILogger<TaskAssignmentController> logger)
        {
            _taskAssignmentService = taskAssignmentService;
            _logger = logger;
        }

        [HttpGet("{taskId}", Name = "GetTaskAssignmentByTaskId")]
        public async Task<IActionResult> GetTaskAssignmentById([FromRoute] int taskId)
        {
            try
            {
                var taskAssignment = await _taskAssignmentService.GetTaskAssignmentByIdAsync(taskId);
                if (taskAssignment == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Task assignment not found."));
                }

                return Ok(ApiResponse.SuccessResponse(taskAssignment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting task assignment for task ID {taskId}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving task assignment"));
            }
        }


        [HttpPost("{taskId}")]
        public async Task<IActionResult> AssignUserToTask([FromRoute] int taskId, [FromBody] AddTaskAssignmentRequestDto taskAssignmentRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                }

                var currentUser = this.GetUserId();
                if (string.IsNullOrEmpty(currentUser))
                {
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
                }

                var result = await _taskAssignmentService.AssignUserToTaskAsync(taskId, currentUser, taskAssignmentRequestDto);

                if (result == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Task not found."));
                }

                return CreatedAtAction("GetTaskAssignmentByTaskId",
                    new { taskId = result.UserId },
                    ApiResponse.SuccessResponse(result, "Comment created successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to assign users to this task."));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning the user to task with task ID{taskId}");
                return StatusCode(500, ApiResponse.ErrorResponse($"An error occurred while assigning a user to a task "));
            }

        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateAssignedUsers([FromRoute] int taskId, [FromBody] UpdateTaskAssignmentRequestDto updateTaskAssignmentRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                }
                var currentUser = this.GetUserId();
                if (string.IsNullOrEmpty(currentUser))
                {
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
                }
                var result = await _taskAssignmentService.UpdateAssignedUsersAsync(taskId, currentUser, updateTaskAssignmentRequestDto);
                if (result == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Task not found."));
                }
                return Ok(ApiResponse.SuccessResponse(result, "Assigned users updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to update assigned users for this task."));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating assigned users for task with ID {taskId}");
                return StatusCode(500, ApiResponse.ErrorResponse($"An error occurred while updating assigned users for the task"));
            }
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> RemoveAssignment([FromRoute] int taskId, [FromBody] string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                }
                var currentUser = this.GetUserId();
                if (string.IsNullOrEmpty(currentUser))
                {
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
                }
                var result = await _taskAssignmentService.RemoveAssignmentsAsync(taskId, currentUser);
                if (result == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Task not found."));
                }
                return Ok(ApiResponse.SuccessResponse(result, "Assignment removed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to remove assignments for this task."));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing assignment for task with ID {taskId}");
                return StatusCode(500, ApiResponse.ErrorResponse($"An error occurred while removing assignment for the task"));
            }
        }
    }
}