using Task_Management.Models;

namespace Task_Management.Models.Tasks
{
    public class PersonalTask : TaskBase
    {
        public bool IsRecurring { get; set; }
        public string RecurrencePattern { get; set; }

        public override int CalculatePriority()
        {
            return IsRecurring ? 6 : 4;
        }

        public override int GetXPReward() => 30;
    }
}