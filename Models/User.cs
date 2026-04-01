namespace Task_Management.Models
{
    public class User
    {
        public User(string email, string passwordHash, string name)
        {
            Email = email;
            PasswordHash = passwordHash;
            Name = name;
            CreatedAt = DateTime.UtcNow;
        }

        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public bool IsPremium { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}