using Task_Management.Models;
using Task_Management.Models.Tasks;

namespace Task_Management.Patterns.Creational.AbstractFactory
{
    public class PersonalFactory : ITaskFamilyFactory
    {
        public TaskBase CreateTask(string text)
        {
            return new PersonalTask
            {
                Id = DateTime.Now.Millisecond + DateTime.Now.Second * 1000,
                Text = text,
                Category = TaskCategory.Personal,
                CreatedAt = DateTime.Now,
                Stage = TaskStage.Todo
            };
        }

        public PersonalNotes CreateNotes()
        {
            var notes = new PersonalNotes(TaskCategory.Personal);
            notes.Content = "🏠 Note personal:\n- Daily tasks\n- Habits tracker\n- Personal goals";
            return notes;
        }

        public UserProgress CreateProgress()
        {
            return new UserProgress
            {
                Xp = 30,
                UnlockedFeatures = new Dictionary<string, bool>
                {
                    { "calendar_integration", false },
                    { "advanced_notes", false }
                }
            };
        }
    }
}