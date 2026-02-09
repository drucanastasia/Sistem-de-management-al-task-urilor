
using Task_Management.Models;

namespace Task_Management.Repository
{
    public interface ITask
    {
        void AddTask(TaskItem task);
        void UpdateTask(TaskItem task);
        void DeleteTask(int id);
        System.Collections.Generic.List<TaskItem> GetTasksByCategory(TaskCategory category);
        void MoveTaskToStage(int id, TaskStage newStage);
        void SaveNotes(TaskCategory category, string content);
        string GetNotes(TaskCategory category);
    }
}