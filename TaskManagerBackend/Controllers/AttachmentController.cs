using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AttachmentController(IAttachmentService attachmentService, IWebHostEnvironment webHostEnvironment)
    {
        _attachmentService = attachmentService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetAllByTaskId(int taskId)
    {
        var attachments = await _attachmentService.GetAttachmentsByTaskIdAsync(taskId);
        return Ok(ApiResponse.SuccessResponse(attachments));
    }

    [HttpGet("download/{attachmentId}")]
    public async Task<IActionResult> Download([FromRoute] int attachmentId)
    {
        var result = await _attachmentService.DownloadAttachmentAsync(attachmentId, _webHostEnvironment.ContentRootPath);

        return File(result.FileBytes, result.ContentType, result.FileName);
    }


    [HttpPost("{taskId}/upload")]
    public async Task<IActionResult> Upload(int taskId, [FromForm] UploadAttachmentRequestDto request)
    {
        var userId = this.GetUserId();
        var uploadedFile = await _attachmentService.UploadAsync(request, userId, taskId);
        return Ok(ApiResponse.SuccessResponse(uploadedFile, "File uploaded successfully"));
    }

    [HttpDelete("{attachmentId}")]
    public async Task<IActionResult> Delete(int attachmentId)
    {
        var userId = this.GetUserId();
        await _attachmentService.DeleteAttachmentAsync(attachmentId, userId);
        return Ok(ApiResponse.SuccessResponse(null, "Attachment deleted successfully"));
    }
}
