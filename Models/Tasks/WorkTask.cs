using Task_Management.Models;

namespace Task_Management.Models.Tasks
{
    public class WorkTask : TaskBase
    {
        public string Client { get; set; }
        public decimal Budget { get; set; }

        public override int CalculatePriority()
        {
            if (Budget >= 5000) return 10;
            if (Budget >= 1000) return 8;
            if (Budget >= 500) return 6;
            return 4;
        }

        public override int GetXPReward() => 75;
    }
}