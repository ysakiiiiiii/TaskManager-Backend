using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public class LocalAttachmentRepository : IAttachmentRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TaskDbContext dbContext;

        public LocalAttachmentRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, TaskDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<List<Attachment>> GetByTaskIdAsync(int taskId)
        {
            return await dbContext.Attachments
                .Where(a => a.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<Attachment?> GetByIdAsync(int id)
        {
            return await dbContext.Attachments.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task DeleteAsync(Attachment attachment)
        {
            var fileNameWithExtension = attachment.FileName + attachment.FileExtension;

            var localFilePath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "UploadedAttachments",
                fileNameWithExtension
            );

            // Remove physical file
            if (File.Exists(localFilePath))
                File.Delete(localFilePath);

            dbContext.Attachments.Remove(attachment);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Attachment> UploadAsync(Attachment attachment, int taskId)
        {
            var fileNameWithExtension = attachment.FileName + attachment.FileExtension;
            var localFilePath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "UploadedAttachments",
                fileNameWithExtension
            );

            //Upload Image to Local Path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await attachment.File.CopyToAsync(stream);

            // https://localhost:1234/Attachments/attachment.extension
            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Attachments/{attachment.FileName}{attachment.FileExtension}";

            attachment.FilePath = urlFilePath;

            await dbContext.Attachments.AddAsync(attachment);
            await dbContext.SaveChangesAsync();

            return attachment;

        }

    }
}
