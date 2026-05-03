namespace Task_Management.Models
{
    public class Note
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
}