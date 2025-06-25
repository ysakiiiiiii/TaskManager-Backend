namespace TaskManagerBackend.Models.Domain
{
    public class Attachment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string UploadedById { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;

        //Foreing Keys
        public TaskItem Task { get; set; }
        public User UploadedBy { get; set; }
    }
}
