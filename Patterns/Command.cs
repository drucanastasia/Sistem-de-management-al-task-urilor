namespace Task_Management.Models.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class AddTaskCommand : ICommand
    {
        private List<TaskItem> _taskList;
        private TaskItem _task;

        public AddTaskCommand(List<TaskItem> taskList, TaskItem task)
        {
            _taskList = taskList;
            _task = task;
        }

        public void Execute()
        {
            _taskList.Add(_task);
        }

        public void Undo()
        {
            _taskList.Remove(_task);
        }
    }

    public class DeleteTaskCommand : ICommand
    {
        private List<TaskItem> _taskList;
        private TaskItem _task;
        private int _index;

        public DeleteTaskCommand(List<TaskItem> taskList, TaskItem task)
        {
            _taskList = taskList;
            _task = task;
            _index = taskList.IndexOf(task);
        }

        public void Execute()
        {
            _taskList.Remove(_task);
        }

        public void Undo()
        {
            _taskList.Insert(_index, _task);
        }
    }

    public class UpdateTaskStatusCommand : ICommand
    {
        private TaskItem _task;
        private string _oldStatus;
        private string _newStatus;

        public UpdateTaskStatusCommand(TaskItem task, string newStatus)
        {
            _task = task;
            _oldStatus = task.Status;
            _newStatus = newStatus;
        }

        public void Execute()
        {
            _task.Status = _newStatus;
        }

        public void Undo()
        {
            _task.Status = _oldStatus;
        }
    }

    public class CommandManager
    {
        private Stack<ICommand> _history = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _history.Push(command);
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                var command = _history.Pop();
                command.Undo();
            }
        }

        public bool CanUndo => _history.Count > 0;
    }
}

