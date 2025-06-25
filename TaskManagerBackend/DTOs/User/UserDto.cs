namespace TaskManagerBackend.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }        
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public DateTime UserCreated { get; set; }
        public DateTime? UserModified { get; set; }
    }
}
