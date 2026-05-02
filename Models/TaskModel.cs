namespace Task_Management.Models
{
    public class TaskModel
    {
        public int Id{  get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string State { get; set; }
    }
}
