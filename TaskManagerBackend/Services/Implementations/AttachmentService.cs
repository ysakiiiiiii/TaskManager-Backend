using AutoMapper;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories.Interfaces;
using TaskManagerBackend.Services.Interfaces;

namespace TaskManagerBackend.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository attachmentRepository;
        private readonly IMapper mapper;
        private readonly ITaskRepository _taskRepository;

        public AttachmentService(IAttachmentRepository attachmentRepository, IMapper mapper, ITaskRepository taskRepository)
        {
            this.attachmentRepository = attachmentRepository;
            this.mapper = mapper;
            _taskRepository = taskRepository;
        }
        public async Task<List<AttachmentDto>> GetAttachmentsByTaskIdAsync(int taskId)
        {
            var attachments = await attachmentRepository.GetByTaskIdAsync(taskId);
            return attachments.Select(a => mapper.Map<AttachmentDto>(a)).ToList();
        }

        public async Task<AttachmentDto> GetAttachmentByIdAsync(int id)
        {
            var attachment = await attachmentRepository.GetByIdAsync(id);
            return attachment is null ? null : mapper.Map<AttachmentDto>(attachment);
        }

        public async Task<string> GetFilePathAsync(int id)
        {
            var attachment = await attachmentRepository.GetByIdAsync(id);
            return attachment?.FilePath;
        }

        public async Task DeleteAttachmentAsync(int id, string userId)
        {
            var attachment = await attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
                throw new FileNotFoundException("Attachment not found");
            if (attachment.UploadedById != userId)
                throw new UnauthorizedAccessException("Access denied.");

            await attachmentRepository.DeleteAsync(attachment);
        }


        public async Task<AttachmentDto> UploadAsync(UploadAttachmentRequestDto request, string userId, int taskId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (!task.AssignedUsers.Any(u => u.UserId == userId))
            {
                throw new UnauthorizedAccessException("Access denied.");
            }


            var attachment = new Attachment
            {
                TaskId = taskId,
                UploadedById = userId,
                File = request.File,
                FileName = request.FileName.Trim().ToLowerInvariant(), 
                ContentType = request.File.ContentType,
                FileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant(),
                FileSizeInBytes = request.File.Length,
                DateUploaded = DateTime.UtcNow
            };

            attachment = await attachmentRepository.UploadAsync(attachment, taskId);

            return mapper.Map<AttachmentDto>(attachment);





        }
    }
}
