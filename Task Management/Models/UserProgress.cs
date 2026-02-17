namespace Task_Management.Models
{
    public class UserProgress
    {
        public int Xp { get; set; }
        public Dictionary<string, bool> UnlockedFeatures { get; set; } = new();
    }
}