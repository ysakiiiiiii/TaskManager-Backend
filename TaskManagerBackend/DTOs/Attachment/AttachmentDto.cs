namespace TaskManagerBackend.DTOs.Attachment
{
    /// <summary>
    /// Represents an attachment response DTO
    /// </summary>
    public sealed record AttachmentDto
    {
        public int Id { get; init; }
        public string FileName { get; init; }
        public string FileExtension { get; init; }
        public string FilePath { get; init; }
        public DateTime DateUploaded { get; init; }
        public string UploadedById { get; init; }
    }
}
