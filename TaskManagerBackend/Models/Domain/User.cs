﻿namespace TaskManagerBackend.Models.Domain
{
    public class User
    {   
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime UserCreated { get; set; }
        public DateTime? UserModified { get; set; }

    }
}
