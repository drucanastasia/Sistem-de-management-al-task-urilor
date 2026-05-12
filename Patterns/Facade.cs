using Task_Management.Interface;
using Task_Management.Models;
using Task_Management.Models.Commands;
using Task_Management.Models.Observers;
using System.Text.Json;
using Task_Management.Patterns;


namespace Task_Management.Patterns
{
    public class TaskFacade
    {
        public static List<TaskItem> _tasks = new List<TaskItem>();
        private readonly TemplateManager _templates;
        private readonly IFileAdapter? _fileAdapter;
        private readonly List<TaskGroup> _groups = new List<TaskGroup>();
        private readonly CommandManager _commandManager = new CommandManager();
        private readonly LevelSubject _levelSubject = new LevelSubject();
        private readonly Dictionary<string, TaskContext> _taskContexts = new Dictionary<string, TaskContext>();
        private IExportStrategy? _exportStrategy;

        public TaskFacade()
        {
            _templates = new TemplateManager();
            _templates.LoadDefaults();
            _levelSubject.Attach(new ConsoleLevelObserver());
            _levelSubject.Attach(new AchievementObserver());
            _levelSubject.Attach(new NotificationLoggerObserver());
        }

        public TaskFacade(IFileAdapter fileAdapter) : this()
        {
            _fileAdapter = fileAdapter;
        }

       
    


        public TaskItem CreateTask(string title, string category, string descriprion)
        {
            ITaskFactory factory = category.ToLower() switch
            {
                "work" or "munca"       => new WorkTaskFactory(),
                "travel" or "calatorie" => new TravelTaskFactory(),
                "freetime" or "timp"    => new FreeTimeTaskFactory(),
                "rest" or "odihna"      => new RestTaskFactory(),
                _                       => new FreeTimeTaskFactory()
            };
            var task = factory.CreateTask(title, descriprion);
            _tasks.Add(task);
            _taskContexts[task.Id] = new TaskContext(task);
            return task;
        }

        public TaskItem CreateTaskWithBuilder(string title, string category,
                                              string description, int xp = 10)
        {
            var task = new TaskItemBuilder()
                .SetTitle(title)
                .SetCategory(category)
                .SetDescription(description)
                .SetStatus("todo")
                .SetXP(xp)
                .Build();

            task.Id = Guid.NewGuid().ToString();
            task.CreatedAt = DateTime.Now;

            _tasks.Add(task);
            _taskContexts[task.Id] = new TaskContext(task);
            return task;
        }

        public TaskItem? CreateTaskFromTemplate(string templateKey)
        {
            var task = _templates.CreateFromTemplate(templateKey);
            if (task == null) return null;

            _tasks.Add(task);
            _taskContexts[task.Id] = new TaskContext(task);
            return task;
        }

        public void AddTemplate(string key, TaskTemplate template)
            => _templates.AddTemplate(key, template);

        public CalendarEvent CreateEvent(string title, DateTime date, string color)
            => Factory_Method.CreateEvent(title, date, color);

        public ICalendarEvent DecorateEvent(ICalendarEvent calendarEvent, params string[] decorators)
        {
            foreach (var decorator in decorators)
            {
                calendarEvent = decorator.ToLower() switch
                {
                    "urgent"  => new UrgentEventDecorator(calendarEvent),
                    "work"    => new WorkEventDecorator(calendarEvent),
                    "relax"   => new RelaxEventDecorator(calendarEvent),
                    "exam"    => new ExamEventDecorator(calendarEvent),
                    "holiday" => new HolidayEventDecorator(calendarEvent),
                    var d when d.StartsWith("detailed:") =>
                        new DetailedEventDecorator(calendarEvent, d["detailed:".Length..]),
                    _ => calendarEvent
                };
            }
            return calendarEvent;
        }

        public bool AdvanceTaskState(string id)
        {
            if (!_taskContexts.TryGetValue(id, out var ctx)) return false;
            ctx.Next();
            return true;
        }

        public bool RevertTaskState(string id)
        {
            if (!_taskContexts.TryGetValue(id, out var ctx)) return false;
            ctx.Previous();
            return true;
        }

        public string? GetTaskState(string id)
            => _taskContexts.TryGetValue(id, out var ctx) ? ctx.GetStatus() : null;

        public TaskItem AddTaskWithCommand(string title, string category, string description)
        {
            ITaskFactory factory = category.ToLower() switch
            {
                "work" or "munca"       => new WorkTaskFactory(),
                "travel" or "calatorie" => new TravelTaskFactory(),
                "freetime" or "timp"    => new FreeTimeTaskFactory(),
                "rest" or "odihna"      => new RestTaskFactory(),
                _                       => new FreeTimeTaskFactory()
            };
            var task = factory.CreateTask(title, description);
            _taskContexts[task.Id] = new TaskContext(task);

            var command = new AddTaskCommand(_tasks, task);
            _commandManager.ExecuteCommand(command);
            return task;
        }

        public bool DeleteTaskWithCommand(string id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            var command = new DeleteTaskCommand(_tasks, task);
            _commandManager.ExecuteCommand(command);
            return true;
        }

        public bool UpdateStatusWithCommand(string id, string newStatus)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            var command = new UpdateTaskStatusCommand(task, newStatus);
            _commandManager.ExecuteCommand(command);
            return true;
        }

        public bool UndoLastCommand()
        {
            if (!_commandManager.CanUndo) return false;
            _commandManager.Undo();
            return true;
        }

        public bool CanUndo => _commandManager.CanUndo;

        public void AttachObserver(ILevelObserver observer)
            => _levelSubject.Attach(observer);

        public void DetachObserver(ILevelObserver observer)
            => _levelSubject.Detach(observer);

        public bool CompleteTaskWithXP(string id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            task.Status = "done";
            if (_taskContexts.TryGetValue(id, out var ctx))
            {
                while (ctx.GetStatus() != "done")
                    ctx.Next();
            }

            _levelSubject.NotifyTaskCompleted(task.Title, task.XP);
            _levelSubject.AddXP(task.XP);
            return true;
        }

        public int GetCurrentLevel() => _levelSubject.CurrentLevel;
        public int GetTotalXP()      => _levelSubject.TotalXP;

        public void SetExportStrategy(string format)
        {
            _exportStrategy = format.ToLower() switch
            {
                "json" => new JSONExportStrategy(),
                "txt"  => new TXTExportStrategy(),
                "csv"  => new CSVExportStrategy(),
                _      => new JSONExportStrategy()
            };
        }

        public void SetExportStrategy(IExportStrategy strategy)
            => _exportStrategy = strategy;

        public string? ExportUserData(User user)
        {
            if (_exportStrategy == null) return null;
            return _exportStrategy.Export(user);
        }

        public string GetExportExtension()
            => _exportStrategy?.FileExtension ?? "N/A";

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
            catch { return false; }
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
                            _taskContexts[task.Id] = new TaskContext(task);
                        }
                    }
                    return true;
                }
                return false;
            }
            catch { return false; }
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
                _levelSubject.AddXP(totalXP);
                System.Diagnostics.Debug.WriteLine(
                    $"Grup '{group.GetTitle()}' completat! +{totalXP} XP (inclusiv bonus +30)");
                return totalXP;
            }
            return 0;
        }

        public List<TaskGroup> GetAllGroups() => _groups;
    }
}
