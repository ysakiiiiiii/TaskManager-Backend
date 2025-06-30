using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Services;
using TaskManagerBackend.Helpers;
using Microsoft.AspNetCore.Hosting;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService attachmentService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ITaskRepository taskRepository;

        public AttachmentController(IAttachmentService attachmentService, IWebHostEnvironment webHostEnvironment, ITaskRepository taskRepository)
        {
            this.attachmentService = attachmentService;
            this.webHostEnvironment = webHostEnvironment;
            this.taskRepository = taskRepository;
        }


        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetAllByTaskId([FromRoute] int taskId)
        {
            var attachments = await attachmentService.GetAttachmentsByTaskIdAsync(taskId);
            return Ok(attachments);
        }


        [HttpGet("download/{attachmentId}")]
        public async Task<IActionResult> Download([FromRoute] int attachmentId)
        {
            var attachment = await attachmentService.GetAttachmentByIdAsync(attachmentId);
            if (attachment == null)
                return NotFound();
            var filePath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "UploadedAttachments",
                attachment.FileName + attachment.FileExtension
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "File not found on disk." });

            var contentType = FileHelper.GetContentType(attachment.FileName + attachment.FileExtension);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, contentType, attachment.FileName + attachment.FileExtension);
        }


        //POST api/attachment/upload
        [HttpPost("{taskId}/Upload")]
        public async Task<IActionResult> Upload([FromRoute] int taskId, [FromForm] UploadAttachmentRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { message = "You're not authorized." });


            var task = await taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                return NotFound(new { message = "Task not found." });

            // Validate before calling the service
            var validationResult = await ValidateFileUploadAsync(request);
            if (!validationResult)
                return BadRequest(ModelState);

            var uploadedFile = await attachmentService.UploadAsync(request, userId, taskId);
            return Ok(uploadedFile);
        }

        [HttpDelete("{attachmentId}")]
        public async Task<IActionResult> Delete([FromRoute] int attachmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await attachmentService.DeleteAttachmentAsync(attachmentId, userId);
                return Ok(new { message = "Attachment deleted." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch
            {
                return NotFound();
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
}
