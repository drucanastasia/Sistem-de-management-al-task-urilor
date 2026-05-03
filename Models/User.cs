namespace Task_Management.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int XP { get; set; } = 0;
        public int Level => XP / 100 + 1; 
        public List<TaskItem> Tasks { get; set; } = new();
        public List<Note> Notes { get; set; } = new();
        public List<CalendarEvent> Events { get; set; } = new();
    }
}