using Task_Management.Models;

namespace Task_Management.Patterns
{
   
    public interface ITaskState
    {
        string StatusName { get; }
        void MoveToNext(TaskContext context);
        void MoveToPrevious(TaskContext context);
    }

    public class ToDoState : ITaskState
    {
        public string StatusName => "todo";

        public void MoveToNext(TaskContext context)
        {
            context.SetState(new InProgressState());
        }

        public void MoveToPrevious(TaskContext context)
        {
            
        }
    }

    public class InProgressState : ITaskState
    {
        public string StatusName => "prog";

        public void MoveToNext(TaskContext context)
        {
            context.SetState(new DoneState());
        }

        public void MoveToPrevious(TaskContext context)
        {
            context.SetState(new ToDoState());
        }
    }

    public class DoneState : ITaskState
    {
        public string StatusName => "done";

        public void MoveToNext(TaskContext context)
        {
            
        }

        public void MoveToPrevious(TaskContext context)
        {
            context.SetState(new InProgressState());
        }
    }

    public class TaskContext
    {
        private ITaskState _state;
        private TaskItem _task;

        public TaskContext(TaskItem task)
        {
            _task = task;
            _state = new ToDoState();
        }

        public void SetState(ITaskState state)
        {
            _state = state;
            _task.Status = state.StatusName;
        }

        public string GetStatus() => _state.StatusName;

        public void Next() => _state.MoveToNext(this);
        public void Previous() => _state.MoveToPrevious(this);
    }

}
