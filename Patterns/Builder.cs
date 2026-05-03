using Task_Management.Models;
using Task_Management.Enum;
namespace Task_Management.Patterns
{
    public class TaskItemBuilder
    {
        private TaskItem _task = new TaskItem();

        public TaskItemBuilder SetTitle(string title)
        {
            _task.Title = title;
            return this;
        }

        public TaskItemBuilder SetCategory(string category)
        {
            _task.Category = category;
            return this;
        }

        public TaskItemBuilder SetDescription(string description)
        {
            _task.Description = description;
            return this;
        }

        public TaskItemBuilder SetStatus(string status)
        {
            _task.Status = TaskState.ToDo;
            return this;
        }

        public TaskItemBuilder SetXP(int xp)
        {
            _task.XP = xp;
            return this;
        }

        public TaskItem Build()
        {
            return _task;
        }
    }
}