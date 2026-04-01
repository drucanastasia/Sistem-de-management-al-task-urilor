using Task_Management.Models;

namespace Task_Management.Models.Tasks
{
    public class TravelTask : TaskBase
    {
        public string Destination { get; set; }
        public DateTime TravelDate { get; set; }

        public override int CalculatePriority()
        {
            var daysUntil = (TravelDate - DateTime.Now).Days;
            if (daysUntil <= 7) return 8;
            if (daysUntil <= 30) return 5;
            return 2;
        }

        public override int GetXPReward() => 25;
    }
}