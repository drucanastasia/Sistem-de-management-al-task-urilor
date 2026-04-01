using Task_Management.Models;
using Task_Management.TaskMove;

namespace Task_Management.Services
{
    public class TaskService
    {
        private ITask _taskmove;

        public TaskService(ITask taskmove)
        {
            _taskmove = taskmove;
        }

        public void AddTask(string text, TaskCategory category)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new System.ArgumentException("Textul task-ului nu poate fi gol.");

            int id = System.DateTime.Now.Millisecond +
                     System.DateTime.Now.Second * 1000 +
                     System.DateTime.Now.Minute * 100000;

            TaskItem task = new TaskItem(id, text, category);
            _taskmove.AddTask(task);
        }

        public void MoveTask(int id, TaskStage newStage)
        {
            _taskmove.MoveTaskToStage(id, newStage);
        }

        public void UpdateTask(int id, string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new System.ArgumentException("Textul nou nu poate fi gol.");

            System.Collections.Generic.List<TaskItem> allTasks =
                new System.Collections.Generic.List<TaskItem>();

            foreach (TaskCategory cat in System.Enum.GetValues(typeof(TaskCategory)))
            {
                allTasks.AddRange(_taskmove.GetTasksByCategory(cat));
            }

            TaskItem taskToUpdate = null;
            foreach (TaskItem task in allTasks)
            {
                if (task.Id == id)
                {
                    taskToUpdate = task;
                    break;
                }
            }

            if (taskToUpdate != null)
            {
                taskToUpdate.Text = newText;
                _taskmove.UpdateTask(taskToUpdate);
            }
        }

        public void DeleteTask(int id)
        {
            _taskmove.DeleteTask(id);
        }

        public System.Collections.Generic.List<TaskItem> GetTasks(TaskCategory category)
        {
            return _taskmove.GetTasksByCategory(category);
        }

        public void SaveNotes(TaskCategory category, string content)
        {
            _taskmove.SaveNotes(category, content);
        }

        public string GetNotes(TaskCategory category)
        {
            return _taskmove.GetNotes(category);
        }
    }
}