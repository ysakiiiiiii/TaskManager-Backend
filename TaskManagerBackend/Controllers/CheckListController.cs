using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.Services;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckListController : ControllerBase
    {
        private readonly ICheckListService checkListService;

        public CheckListController(ICheckListService checkListService)
        {
            this.checkListService = checkListService;
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetCheckListByTask([FromRoute] int taskId)
        {
            var checkList = await checkListService.GetCheckListByTaskAsync(taskId);
            return Ok(checkList);
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> CreateCheckList([FromRoute] int taskId, [FromBody] AddCheckListItemDto addCheckListItemDto)
        {
            if (addCheckListItemDto == null)
            {
                return BadRequest("CheckList cannot be null");
            }
            var createdCheckList = await checkListService.CreateCheckListAsync(taskId, addCheckListItemDto);

            return Ok("Checklistt Added Successfully!");
        }



    }
}