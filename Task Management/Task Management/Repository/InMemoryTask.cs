
using Task_Management.Repository;
using Task_Management.Models;

namespace Task_Management.Interface
{
    public class InMemoryTaskInterface : ITask
    {
        private System.Collections.Generic.List<TaskItem> _tasks;
        private System.Collections.Generic.Dictionary<TaskCategory, PersonalNotes> _notes;

        public InMemoryTaskInterface()
        {
            _tasks = new System.Collections.Generic.List<TaskItem>();
            _notes = new System.Collections.Generic.Dictionary<TaskCategory, PersonalNotes>();
        }

        public void AddTask(TaskItem task)
        {
            _tasks.Add(task);
        }

        public void UpdateTask(TaskItem task)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].Id == task.Id)
                {
                    _tasks[i] = task;
                    return;
                }
            }
        }

        public void DeleteTask(int id)
        {
            _tasks.RemoveAll(t => t.Id == id);
        }

        public System.Collections.Generic.List<TaskItem> GetTasksByCategory(TaskCategory category)
        {
            System.Collections.Generic.List<TaskItem> result = new System.Collections.Generic.List<TaskItem>();
            foreach (TaskItem task in _tasks)
            {
                if (task.Category == category)
                {
                    result.Add(task);
                }
            }
            return result;
        }

        public void MoveTaskToStage(int id, TaskStage newStage)
        {
            foreach (TaskItem task in _tasks)
            {
                if (task.Id == id)
                {
                    task.Stage = newStage;
                    break;
                }
            }
        }

        public void SaveNotes(TaskCategory category, string content)
        {
            if (_notes.ContainsKey(category))
            {
                _notes[category].Content = content;
            }
            else
            {
                PersonalNotes notes = new PersonalNotes(category);
                notes.Content = content;
                _notes.Add(category, notes);
            }
        }

        public string GetNotes(TaskCategory category)
        {
            if (_notes.ContainsKey(category))
            {
                return _notes[category].Content;
            }
            return string.Empty;
        }
    }

    public interface ITaskInterface
    {
    }
}
