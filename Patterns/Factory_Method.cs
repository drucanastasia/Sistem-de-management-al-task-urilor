
using Task_Management.Models;
namespace Task_Management.Patterns
{

    public class Factory_Method
    {
        private const int XP_task = 10;
        public static TaskItem CreateTask(string title, string category,string description)
        {
            return new TaskItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Category = category,
                Description = description,
                Status = "todo" ,
                XP = XP_task,
                CreatedAt = DateTime.Now
            };
        }
    
public static CalendarEvent CreateEvent(string title,  DateTime date, string color)
        {
            return new CalendarEvent
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Date = date,
                Color = color,
                CreatedAt = DateTime.Now
            };
        }
    }

}
