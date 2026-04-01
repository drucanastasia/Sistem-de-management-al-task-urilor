using Task_Management.Models;
using Task_Management.Models.Tasks;


namespace Task_Management.Patterns.Creational.AbstractFactory
{
    public class TravelFactory : ITaskFamilyFactory
    {
        public TaskBase CreateTask(string text)
        {
            return new TravelTask
            {
                Id = DateTime.Now.Millisecond + DateTime.Now.Second * 1000,
                Text = text,
                Category = TaskCategory.Travel,
                CreatedAt = DateTime.Now,
                Stage = TaskStage.Todo
            };
        }

        public PersonalNotes CreateNotes()
        {
            var notes = new PersonalNotes(TaskCategory.Travel);
            notes.Content = "Note travel:\n- Destinations\n- Booking info\n- Packing list";
            return notes;
        }

        public UserProgress CreateProgress()
        {
            return new UserProgress
            {
                Xp = 25,
                UnlockedFeatures = new Dictionary<string, bool>
                {
                    { "calendar_integration", false },
                    { "advanced_notes", false }
                }
            };
        }
    }
}