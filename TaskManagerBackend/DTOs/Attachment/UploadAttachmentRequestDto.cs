using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.DTOs.Attachment
{
    /// <summary>
    /// Represents a request DTO for uploading attachments
    /// </summary>
    public sealed record UploadAttachmentRequestDto
    {
        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; init; }

        [Required(ErrorMessage = "File name is required")]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        public string FileName { get; init; }
    }
}
