namespace TaskManagerBackend.DTOs.Attachment
{
    public class DownloadResult
    {
        public byte[] FileBytes { get; set; } = default!;
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

}
