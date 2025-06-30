using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.DTOs.Attachment
{
    public class UploadAttachmentRequestDto
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string FileName { get; set; }
    }
}
