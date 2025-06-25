using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskRepository taskRepository;
        private readonly IMapper mapper;

        public TaskItemController(ITaskRepository taskRepository, IMapper mapper)
        {
            this.taskRepository = taskRepository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            var taskDto = mapper.Map<TaskDto>(task);
            return Ok(taskDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] AddTaskRequestDto requestDto)
        {


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var taskItem = new TaskItem
                {
                    Title = requestDto.Title,
                    Description = requestDto.Description,
                    CategoryId = requestDto.CategoryId,
                    PriorityId = requestDto.PriorityId,
                    StatusId = requestDto.StatusId,
                    DueDate = requestDto.DueDate,
                    CreatedById = userId,
                    DateCreated = DateTime.UtcNow,
                };

                // Assigned users via navigation property
                taskItem.AssignedUsers = requestDto.AssignedUsersId
                    .Select(assignedUserId => new TaskAssignment
                    {
                        UserId = assignedUserId,
                        Task = taskItem
                    }).ToList();

                // Checklist items
                taskItem.CheckListItems = requestDto.ChecklistItems
                    .Select(item => new CheckList
                    {
                        Description = item.Description,
                        IsCompleted = false,
                        Task = taskItem
                    }).ToList();
                  
                // Attachments
                if (requestDto.Attachments != null)
                {
                    taskItem.Attachments = requestDto.Attachments
                        .Select(att => new Attachment
                        {
                            FileName = att.FileName,
                            FilePath = att.FilePath,
                            UploadedById = userId,
                            Task = taskItem
                        }).ToList();
                }

                var createdTask = await taskRepository.CreateTaskAsync(taskItem);

                
                var taskDto = mapper.Map<TaskDto>(createdTask);
                return CreatedAtAction(nameof(GetTaskById), new { id = taskDto.Id }, taskDto);

            }

            return BadRequest();
        }

        [Authorize]
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            var claims = User?.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new
            {
                Message = "Claims from token",
                Claims = claims
            });
        }


    }
}
