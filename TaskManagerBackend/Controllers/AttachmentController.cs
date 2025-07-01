using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Helpers;
using TaskManagerBackend.Repositories;
using TaskManagerBackend.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<AttachmentController> _logger;

    public AttachmentController(
        IAttachmentService attachmentService,
        IWebHostEnvironment webHostEnvironment,
        ITaskRepository taskRepository,
        ILogger<AttachmentController> logger)
    {
        _attachmentService = attachmentService;
        _webHostEnvironment = webHostEnvironment;
        _taskRepository = taskRepository;
        _logger = logger;
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetAllByTaskId([FromRoute] int taskId)
    {
        try
        {
            var attachments = await _attachmentService.GetAttachmentsByTaskIdAsync(taskId);
            return Ok(ApiResponse.SuccessResponse(attachments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting attachments for task ID {taskId}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while retrieving attachments"));
        }
    }

    [HttpGet("download/{attachmentId}")]
    public async Task<IActionResult> Download([FromRoute] int attachmentId)
    {
        try
        {
            var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId);
            if (attachment == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Attachment not found"));
            }

            var filePath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                "UploadedAttachments",
                attachment.FileName + attachment.FileExtension
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(ApiResponse.ErrorResponse("File not found on disk"));
            }

            var contentType = FileHelper.GetContentType(attachment.FileName + attachment.FileExtension);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, contentType, attachment.FileName + attachment.FileExtension);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading attachment with ID {attachmentId}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while downloading the file"));
        }
    }

    [HttpPost("{taskId}/Upload")]
    public async Task<IActionResult> Upload([FromRoute] int taskId, [FromForm] UploadAttachmentRequestDto request)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
            }

            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Task not found"));
            }

            var validationResult = await ValidateFileUploadAsync(request);
            if (!validationResult)
            {
                return BadRequest(ApiResponse.ErrorResponse("Invalid file upload", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));
            }

            var uploadedFile = await _attachmentService.UploadAsync(request, userId, taskId);
            return Ok(ApiResponse.SuccessResponse(uploadedFile, "File uploaded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error uploading attachment for task ID {taskId}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while uploading the file"));
        }
    }

    [HttpDelete("{attachmentId}")]
    public async Task<IActionResult> Delete([FromRoute] int attachmentId)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
            }

            await _attachmentService.DeleteAttachmentAsync(attachmentId, userId);
            return Ok(ApiResponse.SuccessResponse(null, "Attachment deleted successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, ApiResponse.ErrorResponse("You are not authorized to delete this attachment"));
        }
        catch (FileNotFoundException)
        {
            return NotFound(ApiResponse.ErrorResponse("Attachment not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting attachment with ID {attachmentId}");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the attachment"));
        }
    }

    private async Task<bool> ValidateFileUploadAsync(UploadAttachmentRequestDto request)
    {
        if (request == null || request.File == null)
        {
            ModelState.AddModelError("File", "File is required.");
            return false;
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx", ".pptx" };
        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError("File", $"Extension '{extension}' is not allowed.");
            return false;
        }

        if (request.File.Length > 10 * 1024 * 1024)
        {
            ModelState.AddModelError("File", "File size exceeds 10 MB.");
            return false;
        }

        var isValidContent = await FileValidator.IsValidFileAsync(request.File, extension);
        if (!isValidContent)
        {
            ModelState.AddModelError("File", $"The file content does not match the extension '{extension}'.");
            return false;
        }

        return true;
    }
}