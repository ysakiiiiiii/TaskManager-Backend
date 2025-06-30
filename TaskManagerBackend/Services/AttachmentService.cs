using AutoMapper;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Models.Domain;
using TaskManagerBackend.Repositories;

namespace TaskManagerBackend.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository attachmentRepository;
        private readonly IMapper mapper;

        public AttachmentService(IAttachmentRepository attachmentRepository, IMapper mapper)
        {
            this.attachmentRepository = attachmentRepository;
            this.mapper = mapper;
        }
        public Task DeleteAttachmentAsync(int id, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<AttachmentDto> GetAttachmentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AttachmentDto>> GetAttachmentsByTaskIdAsync(int taskId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetFilePathAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<AttachmentDto> UploadAsync(UploadAttachmentRequestDto request, string userId, int taskId)
        {       
            var attachment = new Attachment     
            {
                TaskId = taskId,
                UploadedById = userId,
                File = request.File,
                FileName = Path.GetFileNameWithoutExtension(request.File.FileName).ToLowerInvariant(),
                ContentType = request.File.ContentType,
                FileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant(),
                FileSizeInBytes = request.File.Length,
                DateUploaded = DateTime.UtcNow
            };

            attachment= await attachmentRepository.UploadAsync(attachment, taskId);

            return mapper.Map<AttachmentDto>(attachment);





        }
    }
}
