using Microsoft.AspNetCore.Authorization;
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

        public TaskAssignmentController(ITaskAssignmentService taskAssignmentService)
        {
            _taskAssignmentService = taskAssignmentService;
        }

        [HttpGet("{taskId}", Name = "GetTaskAssignmentByTaskId")]
        public async Task<IActionResult> GetTaskAssignmentById([FromRoute] int taskId)
        {
            var taskAssignment = await _taskAssignmentService.GetTaskAssignmentByIdAsync(taskId);
            return Ok(ApiResponse.SuccessResponse(taskAssignment));
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> AssignUserToTask([FromRoute] int taskId, [FromBody] AddTaskAssignmentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.ToErrorList()));

            var userId = this.GetUserId();
            var result = await _taskAssignmentService.AssignUserToTaskAsync(taskId, userId, dto);

            return CreatedAtAction(nameof(GetTaskAssignmentById), new { taskId = result.TaskId },
                ApiResponse.SuccessResponse(result, "User assigned to task successfully"));
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateAssignedUsers([FromRoute] int taskId, [FromBody] UpdateTaskAssignmentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.ToErrorList()));

            var userId = this.GetUserId();
            var result = await _taskAssignmentService.UpdateAssignedUsersAsync(taskId, userId, dto);

            return Ok(ApiResponse.SuccessResponse(result, "Assigned users updated successfully"));
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> RemoveAssignment([FromRoute] int taskId)
        {
            var userId = this.GetUserId();
            var result = await _taskAssignmentService.RemoveAssignmentsAsync(taskId, userId);

            return Ok(ApiResponse.SuccessResponse(result, "Assignment removed successfully"));
        }
    }
}
