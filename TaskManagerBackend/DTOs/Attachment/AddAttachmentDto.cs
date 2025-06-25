using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.DTOs.Attachment
{
    public class AddAttachmentDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
