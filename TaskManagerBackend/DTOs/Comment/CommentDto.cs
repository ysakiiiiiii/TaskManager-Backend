namespace TaskManagerBackend.DTOs.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; } 
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
