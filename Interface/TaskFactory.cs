using Task_Management.Models;

namespace Task_Management.Interface
{

    public interface ITaskFactory
    {
        TaskItem CreateTask(string title, string description = "");
        string CategoryName { get; }
        int GetXPBonus();
    }
}
