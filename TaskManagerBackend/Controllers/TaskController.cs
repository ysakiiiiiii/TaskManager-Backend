using AutoMapper;
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
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository taskRepository;
        private readonly IMapper mapper;

        public TaskController(ITaskRepository taskRepository, IMapper mapper)
        {
            this.taskRepository = taskRepository;
            this.mapper = mapper;
        }

        [HttpPost]

        public async Task<IActionResult> CreateTask([FromBody] AddTaskRequestDto addTaskRequestDto)
        {
            var taskDomainModel = mapper.Map<TaskItem>(addTaskRequestDto);

            taskDomainModel = await taskRepository.CreateAsync(taskDomainModel);

            var taskDto = mapper.Map<TaskDto>(taskDomainModel);

            return Ok(taskDto);
        } 
    }
}
