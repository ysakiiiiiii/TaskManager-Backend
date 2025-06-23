namespace TaskManagerBackend.Models.Domain
{
    public class Attachment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        public string UploadedBy { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }

        public DateTime DateUploaded { get; set; }
    }
}
