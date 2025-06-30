using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Services;
using TaskManagerBackend.Helpers;

namespace TaskManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            this.attachmentService = attachmentService;
        }

        //POST api/attachment/upload
        [HttpPost("{taskId}/Upload")]
        public async Task<IActionResult> Upload([FromRoute] int taskId, [FromForm] UploadAttachmentRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate before calling the service
            var validationResult = await ValidateFileUploadAsync(request);
            if (!validationResult)
                return BadRequest(ModelState);

            var uploadedFile = await attachmentService.UploadAsync(request, userId, taskId);
            return Ok(uploadedFile);
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
