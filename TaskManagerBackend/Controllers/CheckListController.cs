using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CheckListController : ControllerBase
{
    private readonly ICheckListService _checkListService;

    public CheckListController(ICheckListService checkListService)
    {
        _checkListService = checkListService;
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetCheckListByTask([FromRoute] int taskId)
    {
        var checklist = await _checkListService.GetCheckListByTaskAsync(taskId);
        return Ok(ApiResponse.SuccessResponse(checklist));
    }

    [HttpPost("{taskId}")]
    public async Task<IActionResult> CreateCheckList([FromRoute] int taskId, [FromBody] AddCheckListItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid input", ModelState.ToErrorList()));

        var userId = this.GetUserId();
        var created = await _checkListService.CreateCheckListAsync(taskId, userId, dto);
        return Ok(ApiResponse.SuccessResponse(created, "Checklist item(s) added successfully"));
    }

    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateCheckList([FromRoute] int taskId, [FromBody] UpdateCheckListRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid input", ModelState.ToErrorList()));

        var userId = this.GetUserId();
        var updated = await _checkListService.UpdateCheckListAsync(userId, dto);
        return Ok(ApiResponse.SuccessResponse(updated, "Checklist item(s) updated successfully"));
    }



    [HttpDelete("{checkListId}")]
    public async Task<IActionResult> DeleteCheckList([FromRoute] int checkListId)
    {
        var userId = this.GetUserId();
        await _checkListService.DeleteCheckListAsync(userId, checkListId);
        return Ok(ApiResponse.SuccessResponse(null, "Checklist item deleted successfully"));
    }

    [HttpPatch("{checkListId}/toggle")]
    public async Task<IActionResult> ToggleCompletion([FromRoute] int checkListId)
    {
        var userId = this.GetUserId();
        await _checkListService.ToggleIsCompletedAsync(userId, checkListId);
        return Ok(ApiResponse.SuccessResponse(null, "Checklist item toggled successfully"));
    }

}
