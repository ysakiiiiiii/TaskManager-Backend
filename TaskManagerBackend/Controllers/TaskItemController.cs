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
        public async Task<IActionResult> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequestDto updateTaskRequestDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var updatedTask = await taskService.UpdateTaskAsync(id, updateTaskRequestDto, userId);
            if (updatedTask == null)
                return NotFound(new { Message = "Task not found." });
            if (updatedTask.Id == 0)
                return StatusCode(403, new { Message = "You are not authorized to update this comment." });

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await taskService.DeleteTaskAsync(id,userId);

            if (result == null)
                return NotFound(new { Message = "Task not found or you are not authorized." });
            if(result == false)
                return StatusCode(403, new { Message = "You are not authorized to update this comment." });

            return Ok(new { Message = "Task deleted successfully." });
        }
    }
}
