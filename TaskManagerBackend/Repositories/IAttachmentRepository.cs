
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface IAttachmentRepository
    {
        Task<Attachment> UploadAsync(Attachment attachment, int taskId);
        
    }
}
