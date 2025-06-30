
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface IAttachmentRepository
    {
        Task<Attachment> UploadAsync(Attachment attachment, int taskId);
        Task<List<Attachment>> GetByTaskIdAsync(int taskId);
        Task<Attachment?> GetByIdAsync(int attachmentId);
        Task DeleteAsync(Attachment attachment);

    }
}
