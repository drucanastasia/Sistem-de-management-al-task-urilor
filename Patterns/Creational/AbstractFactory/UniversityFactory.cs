using Task_Management.Models;
using Task_Management.Models.Tasks;

namespace Task_Management.Patterns.Creational.AbstractFactory
{
    public class UniversityFactory : ITaskFamilyFactory
    {
        public TaskBase CreateTask(string text)
        {
            return new UniversityTask
            {
                Id = DateTime.Now.Millisecond + DateTime.Now.Second * 1000,
                Text = text,
                Category = TaskCategory.University,
                CreatedAt = DateTime.Now,
                Stage = TaskStage.Todo
            };
        }

        public PersonalNotes CreateNotes()
        {
            var notes = new PersonalNotes(TaskCategory.University);
            notes.Content = "Note universitate:\n- Cursuri importante\n- Termene limită\n- Resurse studiu";
            return notes;
        }

        public UserProgress CreateProgress()
        {
            return new UserProgress
            {
                Xp = 50,
                UnlockedFeatures = new Dictionary<string, bool>
                {
                    { "calendar_integration", false },
                    { "advanced_notes", true }
                }
            };
        }
    }
}