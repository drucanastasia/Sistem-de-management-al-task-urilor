using Task_Management.Models;

namespace Task_Management.Models
{
    public class TaskItem
    {
        private int _id;
        private string _text;
        private TaskCategory _category;
        private TaskStage _stage;
        private System.DateTime _createdAt;

        public TaskItem(int id, string text, TaskCategory category)
        {
            _id = id;
            _text = text;
            _category = category;
            _stage = TaskStage.Todo;
            _createdAt = System.DateTime.Now;
        }

        public int Id
        {
            get { return _id; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public TaskCategory Category
        {
            get { return _category; }
        }

        public TaskStage Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }

        public System.DateTime CreatedAt
        {
            get { return _createdAt; }
        }

        public void MoveToStage(TaskStage newStage)
        {
            _stage = newStage;
        }
    }
}