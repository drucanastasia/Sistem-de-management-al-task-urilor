namespace Task_Management.Models
{
    public class CalendarEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Color { get; set; } = "purple", "blue", "green", "amber", "red"; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
}
