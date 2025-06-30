using TaskManagerBackend.DTOs.Attachment;

namespace TaskManagerBackend.Services
{
    public interface IAttachmentService
    {
        Task<AttachmentDto> UploadAsync(UploadAttachmentRequestDto request, string userId, int taskId);
        Task<AttachmentDto> GetAttachmentByIdAsync(int id);
        Task<IEnumerable<AttachmentDto>> GetAttachmentsByTaskIdAsync(int taskId);
        Task DeleteAttachmentAsync(int id, string userId);
        Task<string> GetFilePathAsync(int id);
    }
}
