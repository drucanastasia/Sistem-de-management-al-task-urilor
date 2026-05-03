using Task_Management.Enum;
namespace Task_Management.Models
{
    public class TaskItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; } // work, travel, freetime, rest
        public string Status { get; set; } = "todo";
        public int XP { get; set; } = 10; // Identic pentru toate
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
}