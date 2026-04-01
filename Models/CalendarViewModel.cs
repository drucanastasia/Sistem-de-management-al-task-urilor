namespace TaskManager.Models
{
    public class CalendarViewModel
    {
        public int CurrentYear { get; set; }
        public bool IsPremium { get; set; }
        public List<CalendarNote> Notes { get; set; } = new List<CalendarNote>();
    }

    public class CalendarNote
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public string UserId { get; set; }
    }
}