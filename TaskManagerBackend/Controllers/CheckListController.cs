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
    private readonly ILogger<CheckListController> _logger;

    public CheckListController(ICheckListService checkListService, ILogger<CheckListController> logger)
    {
        _checkListService = checkListService;
        _logger = logger;
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetCheckListByTask([FromRoute] int taskId)
    {
        try
        {
            var checkList = await _checkListService.GetCheckListByTaskAsync(taskId);
            return Ok(ApiResponse.SuccessResponse(checkList));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting checklist for task ID {taskId}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving the checklist"));
        }
    }

    [HttpPost("{taskId}")]
    public async Task<IActionResult> CreateCheckList([FromRoute] int taskId, [FromBody] AddCheckListItemDto dto)
    {
        try
        {
            var userId = this.GetUserId();
            var validationResult = this.ValidateRequest(dto, userId);
            if (validationResult is not OkResult) return validationResult;

            var createdCheckList = await _checkListService.CreateCheckListAsync(taskId, dto);

            return Ok(ApiResponse.SuccessResponse(createdCheckList, "Checklist item added successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating checklist for task ID {taskId}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while creating the checklist item"));
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCheckList([FromBody] UpdateCheckListDto dto)
    {
        try
        {
            var userId = this.GetUserId();
            var validationResult = this.ValidateRequest(dto, userId);
            if (validationResult is not OkResult) return validationResult;

            var updatedCheckList = await _checkListService.UpdateCheckListAsync(dto);
            if (updatedCheckList == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Checklist item not found"));
            }

            return Ok(ApiResponse.SuccessResponse(updatedCheckList, "Checklist item updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating checklist");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while updating the checklist item"));
        }
    }

    [HttpDelete("{checkListId}")]
    public async Task<IActionResult> DeleteCheckList([FromRoute] int checkListId)
    {
        try
        {
            var userId = this.GetUserId();
            var validationResult = this.ValidateRequest(userId);
            if (validationResult is not OkResult) return validationResult;

            var deletedCheckList = await _checkListService.DeleteCheckListAsync(checkListId);
            if (deletedCheckList == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Checklist item not found"));
            }

            if (!deletedCheckList.Value)
            {
                return BadRequest(ApiResponse.ErrorResponse("Failed to delete checklist item"));
            }

            return Ok(ApiResponse.SuccessResponse(null, "Checklist item deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting checklist with ID {checkListId}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the checklist item"));
        }
    }
}