using Task_Management.Models;
using Task_Management.Models.Tasks;

namespace Task_Management.Patterns.Creational.Factory
{
    public static class TaskFactory
    {
        public static TaskBase CreateTask(TaskCategory category, string text)
        {
            return category switch
            {
                TaskCategory.University => new UniversityTask
                {
                    Id = GenerateId(),
                    Text = text,
                    Category = category,
                    CreatedAt = DateTime.Now,
                    Stage = TaskStage.Todo
                },
                TaskCategory.Work => new WorkTask
                {
                    Id = GenerateId(),
                    Text = text,
                    Category = category,
                    CreatedAt = DateTime.Now,
                    Stage = TaskStage.Todo
                },
                TaskCategory.Travel => new TravelTask
                {
                    Id = GenerateId(),
                    Text = text,
                    Category = category,
                    CreatedAt = DateTime.Now,
                    Stage = TaskStage.Todo
                },
                TaskCategory.Personal => new PersonalTask
                {
                    Id = GenerateId(),
                    Text = text,
                    Category = category,
                    CreatedAt = DateTime.Now,
                    Stage = TaskStage.Todo
                },
                _ => throw new ArgumentException($"Categorie invalidă: {category}")
            };
        }

        private static int GenerateId()
        {
            return DateTime.Now.Millisecond +
                   DateTime.Now.Second * 1000 +
                   DateTime.Now.Minute * 100000 +
                   DateTime.Now.Hour * 10000000;
        }
    }
}