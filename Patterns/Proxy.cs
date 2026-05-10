
namespace Task_Management.Models.Proxy
{
    public interface ITaskProxy
    {
        string Id { get; }
        string Title { get; set; }
        string Category { get; set; }
        string Description { get; set; }
        string Status { get; set; }
        int XP { get; }
        DateTime CreatedAt { get; }
        TaskItem GetRealTask();
    }
    public class TaskValidationProxy : ITaskProxy
    {
        private readonly TaskItem _real;

        public TaskValidationProxy(TaskItem task)
        {
            _real = task;
            ApplyInitialSanitization();
        }

        public string Id => _real.Id;
        public DateTime CreatedAt => _real.CreatedAt;
        public int XP => _real.XP; 

        public string Title
        {
            get => _real.Title;
            set => _real.Title = SanitizeTitle(value, _real.Category);
        }

        public string Category
        {
            get => _real.Category;
            set => _real.Category = value?.Trim().ToLower() ?? "freetime";
        }

        public string Description
        {
            get => _real.Description;
            set => _real.Description = (value?.Trim().Length > 100
                ? value.Trim().Substring(0, 100) + "..."
                : value?.Trim()) ?? "";
        }

        public string Status
        {
            get => _real.Status;
            set
            {
                if (new[] { "todo", "prog", "done" }.Contains(value.ToLower()))
                    _real.Status = value.ToLower();
            }
        }

        public TaskItem GetRealTask() => _real;

        private void ApplyInitialSanitization()
        {
            _real.Category = _real.Category?.Trim().ToLower() ?? "freetime";
            _real.Title = SanitizeTitle(_real.Title, _real.Category);
            _real.Description = _real.Description?.Trim() ?? "";
        }

        private string SanitizeTitle(string title, string category)
        {
            if (string.IsNullOrWhiteSpace(title)) return "Task fără titlu";
            title = title.Trim();
            if (title.Length > 50) title = title.Substring(0, 47) + "...";

            string emoji = category.ToLower() switch
            {
                "work" => "💼 ",
                "travel" => "✈️ ",
                "freetime" => "🎯 ",
                "rest" => "😌 ",
                _ => "📝 "
            };
            return title.StartsWith(emoji) ? title : emoji + title;
        }
    }
}