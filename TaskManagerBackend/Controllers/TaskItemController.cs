using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerBackend.DTOs.Task;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskService taskService;

        public TaskItemController(ITaskService taskService)
        {
            this.taskService = taskService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await taskService.GetAllTasksAsync();
            if (tasks == null || !tasks.Any())
                return NotFound(new { Message = "No tasks found." });

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound(new { Message = "Task not found." });

            return Ok(task);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] AddTaskRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var createdTask = await taskService.CreateTaskAsync(dto, userId);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var updatedTask = await taskService.UpdateTaskAsync(id, dto, userId);
            if (updatedTask == null)
                return NotFound(new { Message = "Task not found." });

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            var result = await taskService.DeleteTaskAsync(id);
            if (!result)
                return NotFound(new { Message = "Task not found or already deleted." });

            return Ok(new { Message = "Task deleted successfully." });
        }
    }
}
