
using Task_Management.Interface;
using Task_Management.Models;
namespace Task_Management.Patterns
{
    public class Facade
    {
        public class TaskFacade
        {
            public static List<TaskItem> _tasks = new();
            private readonly TemplateManager _templates;
            public TaskFacade()
            {
                _templates = new TemplateManager();
                _templates.LoadDefaults();
            
          }
            public TaskItem CreateTask(string title, string category, string descriprion = "")
            {
                ITaskFactory factory = category.ToLower() switch
                {
                    "work" => new WorkTaskFactory(),
                    "travel" => new TravelTaskFactory(),
                    "freetime" => new FreeTimeTaskFactory(),
                    "rest" => new RestTaskFactory(),
                    _ => new FreeTimeTaskFactory()
                };
                var task = factory.CreateTask(title, descriprion);

                _tasks.Add(task);
                return task;
            }
            public List<TaskItem> GetAllTasks() => _tasks;

            public bool DeleteTask(string id)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task != null) { _tasks.Remove(task); return true; }
                return false;
            }
            public bool UpdateStatus(string id, string newStatus)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    task.Status = newStatus;  
                    return true;
                }
                return false;
            }








































































































        }
    }
}
