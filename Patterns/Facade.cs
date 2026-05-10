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

         
            private readonly List<TaskGroup> _groups = new();

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

            public TaskGroup CreateGroup(string title)
            {
                var group = new TaskGroup { Title = title };
                _groups.Add(group);
                return group;
            }

            public bool AddTaskToGroup(TaskGroup group, string taskId)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == taskId);
                if (task == null) return false;

                group.Add(task);
                return true;
            }

            
            public int CompleteGroup(TaskGroup group)
            {
                group.Complete();

                if (group.IsCompleted())
                {
                    int totalXP = group.GetXP(); 
                    System.Diagnostics.Debug.WriteLine(
                        $"Grup '{group.GetTitle()}' completat! +{totalXP} XP (inclusiv bonus +30)");
                    return totalXP;
                }

                return 0;
            }

            public List<TaskGroup> GetAllGroups() => _groups;
        }
    }
}
