using Task_Management.Models;

namespace Task_Management.Models
{
    public class PersonalNotes
    {
        private TaskCategory _category;
        private string _content;

        public PersonalNotes(TaskCategory category)
        {
            _category = category;
            _content = string.Empty;
        }

        public TaskCategory Category
        {
            get { return _category; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
