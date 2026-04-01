using Task_Management.Models;

namespace Task_Management.Models.Tasks
{
    public class UniversityTask : TaskBase
    {
        public DateTime Deadline { get; set; }

        public override int CalculatePriority()
        {
            var daysLeft = (Deadline - DateTime.Now).Days;
            if (daysLeft <= 0) return 10;
            if (daysLeft <= 3) return 9;
            if (daysLeft <= 7) return 7;
            return 4;
        }

        public override int GetXPReward() => 50;
    }
}