namespace TaskManagerBackend.Models.Domain
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public int AssignedTo { get; set; }
        public int CategoryId { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DueDate { get; set; }


    }
}
