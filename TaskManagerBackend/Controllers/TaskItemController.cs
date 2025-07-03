// Controllers/TaskItemController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var tasks = await _taskService.GetAllTasksAsync();
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

            var userId = this.GetUserId();
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

            var userId = this.GetUserId();
            var updatedTask = await _taskService.UpdateTaskAsync(id, dto, userId);

            return Ok(ApiResponse.SuccessResponse(updatedTask, "Task updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            var userId = this.GetUserId();
            await _taskService.DeleteTaskAsync(id, userId);
            return Ok(ApiResponse.SuccessResponse(null, "Task deleted successfully"));
        }
    }
}