namespace TaskManagerBackend.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }

        public DateTime DateCreated { get; set; }
        
        public DateTime? DateUpdated { get; set; }
    }
}
