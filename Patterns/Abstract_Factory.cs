using Task_Management.Enum;
using Task_Management.Interface;
using Task_Management.Models;
namespace Task_Management.Patterns
{
        // Factory pentru Work
        public class WorkTaskFactory : ITaskFactory
        {
            public string CategoryName => "work";

            public TaskItem CreateTask(string title, string description = "")
            {
                return new TaskItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $" {title}",
                    Category = "work",
                    Description = description,
                    Status = "todo",
                    XP = 10 + GetXPBonus(), 
                    CreatedAt = DateTime.Now
                };
            }

            public int GetXPBonus() => 5;
        }

        // Factory pentru Travel
        public class TravelTaskFactory : ITaskFactory
        {
            public string CategoryName => "travel";

            public TaskItem CreateTask(string title, string description = "")
            {
                return new TaskItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"{title}",
                    Category = "travel",
                    Description = description,
                    Status = "todo",
                    XP = 10 + GetXPBonus(),
                    CreatedAt = DateTime.Now
                };
            }

            public int GetXPBonus() => 3; 
        }

        // Factory pentru Free Time
        public class FreeTimeTaskFactory : ITaskFactory
        {
            public string CategoryName => "freetime";

            public TaskItem CreateTask(string title, string description = "")
            {
                return new TaskItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"🎯 {title}",
                    Category = "freetime",
                    Description = description,
                    Status = "todo",
                    XP = 10,
                    CreatedAt = DateTime.Now
                };
            }

            public int GetXPBonus() => 0;
        }

        // Factory pentru Rest
        public class RestTaskFactory : ITaskFactory
        {
            public string CategoryName => "rest";

            public TaskItem CreateTask(string title, string description = "")
            {
                return new TaskItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"{title}",
                    Category = "rest",
                    Description = description,
                    Status = "todo",
                    XP = 10,
                    CreatedAt = DateTime.Now
                };
            }

            public int GetXPBonus() => 0;
        }

        // Factory Producer
        public static class TaskCategoryFactoryProducer
        {
            public static ITaskFactory GetFactory(string category)
            {
                return category.ToLower() switch
                {
                    "work" => new WorkTaskFactory(),
                    "travel" => new TravelTaskFactory(),
                    "freetime" => new FreeTimeTaskFactory(),
                    "rest" => new RestTaskFactory(),
                    _ => new FreeTimeTaskFactory()
                };
            }
        }
    }

