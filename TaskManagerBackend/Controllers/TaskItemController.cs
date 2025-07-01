using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                if (tasks == null || !tasks.Any())
                {
                    return NotFound(ApiResponse.ErrorResponse("No tasks found"));
                }

                return Ok(ApiResponse.SuccessResponse(tasks));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tasks");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving tasks"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Task not found"));
                }

                return Ok(ApiResponse.SuccessResponse(task));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting task with ID {id}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving the task"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] AddTaskRequestDto dto)
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

                var createdTask = await _taskService.CreateTaskAsync(dto, userId);
                return CreatedAtAction(nameof(GetTaskById),
                    new { id = createdTask.Id },
                    ApiResponse.SuccessResponse(createdTask, "Task created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while creating the task"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequestDto dto)
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

                var updatedTask = await _taskService.UpdateTaskAsync(id, dto, userId);

                if (updatedTask == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Task not found"));
                }

                if (updatedTask.Id == 0)
                {
                    return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to update this task"));
                }

                return Ok(ApiResponse.SuccessResponse(updatedTask, "Task updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task with ID {id}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while updating the task"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            try
            {
                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
                }

                var result = await _taskService.DeleteTaskAsync(id, userId);

                if (result == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Task not found"));
                }

                if (result == false)
                {
                    return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to delete this task"));
                }

                return Ok(ApiResponse.SuccessResponse(null, "Task deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting task with ID {id}");
                return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the task"));
            }
        }
    }
}
