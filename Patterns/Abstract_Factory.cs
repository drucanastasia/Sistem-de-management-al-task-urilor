using Task_Management.Enum;
using Task_Management.Interface;
using Task_Management.Models;
namespace Task_Management.Patterns
{
        // Factory pentru Work
        public class WorkTaskFactory : ITaskFactory
        {
            public string CategoryName
        {
            get 
            { 
                return "work"; 
            }
        }

            public TaskItem CreateTask(string title, string description)
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

        public int GetXPBonus()
        {
            return 5;
        }
    }

        // Factory pentru Travel
        public class TravelTaskFactory : ITaskFactory
        {
            public string CategoryName 
        {
            get { return "travel"; }
        }

            public TaskItem CreateTask(string title, string description)
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

        public int GetXPBonus()
        {
            return 3;
        }
    }

        // Factory pentru Free Time
        public class FreeTimeTaskFactory : ITaskFactory
        {
            public string CategoryName 
        {
            get { return "freetime"; }
        }

            public TaskItem CreateTask(string title, string description)
            {
                return new TaskItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $" {title}",
                    Category = "freetime",
                    Description = description,
                    Status = "todo",
                    XP = 10,
                    CreatedAt = DateTime.Now
                };
            }

        public int GetXPBonus()
        {
            return 0;
        }
    }

        // Factory pentru Rest
        public class RestTaskFactory : ITaskFactory
        {
        public string CategoryName
        {
            get { return "rest"; }
        }

        public TaskItem CreateTask(string title, string description)
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

        public int GetXPBonus()
        {
            return 0;
        }
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

