using AutoMapper;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Exceptions;
using TaskManagerBackend.Helpers;
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
                throw new NotFoundException("Attachment not found");
            if (attachment.UploadedById != userId)
                throw new ForbiddenException("You are not allowed to delete this attachment");

            await attachmentRepository.DeleteAsync(attachment);
        }

        public async Task<DownloadResult> DownloadAttachmentAsync(int attachmentId, string contentRootPath)
        {
            var attachment = await attachmentRepository.GetByIdAsync(attachmentId)
                ?? throw new NotFoundException("Attachment not found");

            var fileNameWithExt = attachment.FileName + attachment.FileExtension;
            var filePath = Path.Combine(contentRootPath, "UploadedAttachments", fileNameWithExt);

            if (!File.Exists(filePath))
                throw new NotFoundException("File not found on disk");

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var contentType = FileHelper.GetContentType(fileNameWithExt);

            return new DownloadResult
            {
                FileBytes = fileBytes,
                ContentType = contentType,
                FileName = fileNameWithExt
            };
        }

        public async Task<AttachmentDto> UploadAsync(UploadAttachmentRequestDto request, string userId, int taskId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            if (request == null || request.File == null)
                throw new ArgumentException("No file provided");

            var task = await _taskRepository.GetTaskByIdAsync(taskId)
                ?? throw new NotFoundException("Task not found");

            if (!task.AssignedUsers.Any(u => u.UserId == userId))
                throw new ForbiddenException("You are not allowed to upload to this task");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx", ".pptx" };
            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"File extension '{extension}' is not allowed");

            const long maxFileSize = 10 * 1024 * 1024;
            if (request.File.Length > maxFileSize)
                throw new ArgumentException("File size exceeds the 10 MB limit");

            var isValidContent = await FileValidator.IsValidFileAsync(request.File, extension);
            if (!isValidContent)
                throw new ArgumentException($"The content does not match the file extension '{extension}'");

            var attachment = mapper.Map<Attachment>(request);
            attachment.TaskId = taskId;
            attachment.UploadedById = userId;
            attachment.DateUploaded = DateTime.UtcNow;

            var saved = await attachmentRepository.UploadAsync(attachment, taskId);
            return mapper.Map<AttachmentDto>(saved);
        }

    }
}
