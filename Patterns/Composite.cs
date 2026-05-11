using Task_Management.Interface;

namespace Task_Management.Models
{
    public class TaskGroup : ITaskComponent
    {
        private readonly List<ITaskComponent> _children;
        private const int GROUP_BONUS_XP = 30;
        private const int BONUS_THRESHOLD = 5;

        public string Title { get; set; }

        public TaskGroup()
        {
            _children = new List<ITaskComponent>();
            Title = string.Empty;
        }

        public void Add(ITaskComponent component)
        {
            _children.Add(component);
        }

        public void Remove(ITaskComponent component)
        {
            _children.Remove(component);
        }

        public List<ITaskComponent> GetChildren() { return _children;}

        public string GetTitle(){ return $" {Title} ({_children.Count} taskuri)";} 

        public int GetXP()
        {
            int totalXP = _children.Sum(c => c.GetXP());

            int completedCount = _children.Count(c => c.IsCompleted());
            int bonusTimes = completedCount / BONUS_THRESHOLD;
            totalXP += bonusTimes * GROUP_BONUS_XP;

            return totalXP;
        }

        public bool IsCompleted(){ get { return _children.Count > 0 && _children.All(c => c.IsCompleted());
        }}
        public void Complete()
        {
            foreach (var child in _children)
                child.Complete();
        }
    }
}