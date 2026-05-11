using Task_Management.Models;
using Task_Management.Interface;
namespace Task_Management.Patterns
{
    public class TaskTemplate : IPrototype<TaskItem>
    {
        public string Name { get; set; }
        public  string Category { get; set; }
        public  string DefaultDescription { get; set; }

        public TaskItem Clone()
        {
            return new TaskItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = this.Name,
                Category = this.Category,
                Description = this.DefaultDescription,
                Status = "todo",
                XP = 10,
                CreatedAt = DateTime.Now
            };
        }
    }

    public class TemplateManager
    {
        private Dictionary<string, TaskTemplate> _templates = new();

        public void AddTemplate(string key, TaskTemplate template)
        {
            _templates[key] = template;
        }

        public TaskItem CreateFromTemplate(string key)
        {
            if (_templates.TryGetValue(key, out var template))
            {
                return template.Clone();
            }
            return null;
        }
            public void LoadDefaults()
        {
            AddTemplate("work_meeting", new TaskTemplate
            {
                Name = "Meeting echipă",
                Category = "work",
                DefaultDescription = "Meeting săptămânal"
            });

            AddTemplate("travel_booking", new TaskTemplate
            {
                Name = "Rezervare bilet",
                Category = "travel",
                DefaultDescription = "Rezervare transport/cazare"
            });

            AddTemplate("workout", new TaskTemplate
            {
                Name = "Sală",
                Category = "freetime",
                DefaultDescription = "30 minute antrenament"
            });
            AddTemplate("Pauza", new TaskTemplate
            {
                Name = "Pauza",
                Category = "rest",
                DefaultDescription = "odihna la mare "
            });
        }
    }
}