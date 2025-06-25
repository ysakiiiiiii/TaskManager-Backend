namespace TaskManagerBackend.DTOs.Attachment
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime DateUploaded { get; set; }
        public string UploadedById { get; set; }
    }
}
