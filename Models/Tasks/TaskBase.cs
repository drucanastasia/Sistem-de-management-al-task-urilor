using Task_Management.Models;

namespace Task_Management.Models.Tasks
{
    public abstract class TaskBase
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public TaskCategory Category { get; set; }
        public TaskStage Stage { get; set; }
        public DateTime CreatedAt { get; set; }
        public int XPReward { get; protected set; }

        public abstract int CalculatePriority();
        public abstract int GetXPReward();
    }
}