using Task_Management.Models;

namespace Task_Management.Interfaceuser
{
    public interface IProgress
    {
        Task<UserProgress> GetProgressAsync(string userId);
        Task SaveProgressAsync(string userId, UserProgress progress);
    }
}
