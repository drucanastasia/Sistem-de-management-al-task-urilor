using Task_Management.Models;
using Task_Management.Models.Tasks;

namespace Task_Management.Patterns.Creational.AbstractFactory
{
    public interface ITaskFamilyFactory
    {
        TaskBase CreateTask(string text);
        PersonalNotes CreateNotes();
        UserProgress CreateProgress();
    }
}