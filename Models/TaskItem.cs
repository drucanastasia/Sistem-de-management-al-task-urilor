

using Task_Management.Interface;
using Task_Management.Enum;

namespace Task_Management.Models
{
 
    public class TaskItem : ITaskComponent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "todo";
        public int XP { get; set; } = 10;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ITaskComponent 
        public string GetTitle() => Title;
        public int GetXP() => XP;
        public bool IsCompleted() => Status == "done";
        public void Complete() => Status = "done";
    }
}
