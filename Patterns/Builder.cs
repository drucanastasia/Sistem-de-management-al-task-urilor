using Task_Management.Models;
namespace Task_Management.Patterns
{
    public class Builder
    {
        private TaskItem _task = new TaskItem();
        public Builder SetTitle(string title)
        {
            _task.Title = title;
            return this;
        }
        public Builder SetDate(DateTime date)
        {
            _task.Date = date;
            return this;
        }
        public Builder SetType(string type)
        {
            _task.Type = type;
            return this;
        }
    }
}
