namespace TaskManagerBackend.Models.Domain
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int ChangedBy { get; set; }
        public string FieldChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public  DateTime ChangeDate { get; set; }

    }
}
