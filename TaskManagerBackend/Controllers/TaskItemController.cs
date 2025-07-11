using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.DTOs.Status;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Helpers;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskItemController> _logger;

        public TaskItemController(ITaskService taskService, ILogger<TaskItemController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] TaskQueryParameters filters)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var tasks = await _taskService.GetAllTasksAsync(filters, userId, userRole);
            return Ok(ApiResponse.SuccessResponse(tasks));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(ApiResponse.SuccessResponse(task));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] AddTaskRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var createdTask = await _taskService.CreateTaskAsync(dto, userId);

            return CreatedAtAction(nameof(GetTaskById),
                new { id = createdTask.Id },
                ApiResponse.SuccessResponse(createdTask, "Task created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("Invalid data", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var updatedTask = await _taskService.UpdateTaskAsync(id, dto, userId);

            return Ok(ApiResponse.SuccessResponse(updatedTask, "Task updated successfully"));
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("admin");

            await _taskService.DeleteTaskAsync(id, userId, isAdmin);

            return NoContent();
        }


        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            var updated = await _taskService.UpdateStatusAsync(id, dto.StatusName);
            if (!updated) return NotFound();

            return Ok();
        }


    }
}
