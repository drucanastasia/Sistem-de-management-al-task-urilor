using Task_Management.Interface;
using Task_Management.Models;
using System.Text.Json;

namespace Task_Management.Patterns
{
    public class Facade
    {
        public class TaskFacade
        {
            public static List<TaskItem> _tasks = new();
            private readonly TemplateManager _templates;
            private readonly IFileAdapter? _fileAdapter;

            public TaskFacade()
            {
                _templates = new TemplateManager();
                _templates.LoadDefaults();
            }

            public TaskFacade(IFileAdapter fileAdapter) : this()
            {
                _fileAdapter = fileAdapter;
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

            public bool ExportTasks(string path)
            {
                if (_fileAdapter == null) return false;
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonData = JsonSerializer.Serialize(_tasks, options);
                    _fileAdapter.WriteFile(path + _fileAdapter.GetExtension(), jsonData);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public bool ImportTasks(string path)
            {
                if (_fileAdapter == null) return false;
                try
                {
                    string jsonData = _fileAdapter.ReadFile(path);
                    var importedTasks = JsonSerializer.Deserialize<List<TaskItem>>(jsonData);
                    if (importedTasks != null)
                    {
                        foreach (var task in importedTasks)
                        {
                            if (_tasks.All(t => t.Id != task.Id))
                            {
                                _tasks.Add(task);
                            }
                        }
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }

            public string GetFileFormat() => _fileAdapter?.GetExtension() ?? "N/A";
        }
    }
}