using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.DTOs.Attachment
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string UploadedById { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
