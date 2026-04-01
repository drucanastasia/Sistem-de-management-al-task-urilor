using Task_Management.Models;
using Task_Management.Models.Tasks;

using Task_Management.Models;
using Task_Management.Models.Tasks;

namespace Task_Management.Patterns.Creational.AbstractFactory
{
    public class WorkFactory : ITaskFamilyFactory
    {
        public TaskBase CreateTask(string text)
        {
            return new WorkTask
            {
                Id = DateTime.Now.Millisecond + DateTime.Now.Second * 1000,
                Text = text,
                Category = TaskCategory.Work,
                CreatedAt = DateTime.Now,
                Stage = TaskStage.Todo
            };
        }

        public PersonalNotes CreateNotes()
        {
            var notes = new PersonalNotes(TaskCategory.Work);
            notes.Content = "Note work:\n- Client contacts\n- Project deadlines\n- Meeting notes";
            return notes;
        }

        public UserProgress CreateProgress()
        {
            return new UserProgress
            {
                Xp = 75,
                UnlockedFeatures = new Dictionary<string, bool>
                {
                    { "calendar_integration", true },
                    { "advanced_notes", true }
                }
            };
        }
    }
}