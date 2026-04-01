using Task_Management.Models;

namespace Task_Management.Patterns.Creational.AbstractFactory
{
    public static class TaskFamilyFactory
    {
        public static ITaskFamilyFactory GetFactory(TaskCategory category)
        {
            return category switch
            {
                TaskCategory.University => new UniversityFactory(),
                TaskCategory.Work => new WorkFactory(),
                TaskCategory.Travel => new TravelFactory(),
                TaskCategory.Personal => new PersonalFactory(),
                _ => throw new ArgumentException($"Categorie invalidă: {category}")
            };
        }
    }
}