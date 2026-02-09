using Task_Management.Models;
using Task_Management.Repository;

namespace Task_Management.Services
{
    public class TaskService
    {
        private ITask _repository;

        public TaskService(ITask repository)
        {
            _repository = repository;
        }

        public void AddTask(string text, TaskCategory category)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new System.ArgumentException("Textul task-ului nu poate fi gol.");

            int id = System.DateTime.Now.Millisecond +
                     System.DateTime.Now.Second * 1000 +
                     System.DateTime.Now.Minute * 100000;

            TaskItem task = new TaskItem(id, text, category);
            _repository.AddTask(task);
        }

        public void MoveTask(int id, TaskStage newStage)
        {
            _repository.MoveTaskToStage(id, newStage);
        }

        public void UpdateTask(int id, string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new System.ArgumentException("Textul nou nu poate fi gol.");

            System.Collections.Generic.List<TaskItem> allTasks =
                new System.Collections.Generic.List<TaskItem>();

            foreach (TaskCategory cat in System.Enum.GetValues(typeof(TaskCategory)))
            {
                allTasks.AddRange(_repository.GetTasksByCategory(cat));
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
                _repository.UpdateTask(taskToUpdate);
            }
        }

        public void DeleteTask(int id)
        {
            _repository.DeleteTask(id);
        }

        public System.Collections.Generic.List<TaskItem> GetTasks(TaskCategory category)
        {
            return _repository.GetTasksByCategory(category);
        }

        public void SaveNotes(TaskCategory category, string content)
        {
            _repository.SaveNotes(category, content);
        }

        public string GetNotes(TaskCategory category)
        {
            return _repository.GetNotes(category);
        }
    }
}