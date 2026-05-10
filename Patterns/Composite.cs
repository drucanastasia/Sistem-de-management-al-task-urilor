using Task_Management.Interface;

namespace Task_Management.Models
{
    public class TaskGroup : ITaskComponent
    {
        private readonly List<ITaskComponent> _children = new();
        private const int GROUP_BONUS_XP = 30;

        public string Title { get; set; } = string.Empty;

        public void Add(ITaskComponent component)
        {
            _children.Add(component);
        }

        public void Remove(ITaskComponent component)
        {
            _children.Remove(component);
        }

        public List<ITaskComponent> GetChildren() => _children;

       
        public string GetTitle() => $" {Title} ({_children.Count} taskuri)";

        public int GetXP()
        {
            int totalXP = _children.Sum(c => c.GetXP());

          
            if (IsCompleted())
                totalXP += GROUP_BONUS_XP;

            return totalXP;
        }


        public bool IsCompleted() => _children.Count > 0 && _children.All(c => c.IsCompleted());

        public void Complete()
        {
            foreach (var child in _children)
                child.Complete();
        }
    }
}
