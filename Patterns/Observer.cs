using System;
using System.Collections.Generic;

namespace Task_Management.Models.Observers
{
    public interface ILevelObserver
    {
        void OnLevelUp(int newLevel, int totalXP);
        void OnAchievement(string achievementName);
        void OnTaskCompleted(string taskTitle, int xpGained);
    }

    public class LevelSubject
    {
        private List<ILevelObserver> _observers = new List<ILevelObserver>();
        public int CurrentLevel { get; private set; }
        public int TotalXP { get; private set; }

        public void Attach(ILevelObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(ILevelObserver observer)
        {
            _observers.Remove(observer);
        }

        public void AddXP(int amount)
        {
            int oldLevel = CurrentLevel;
            TotalXP += amount;
            CurrentLevel = TotalXP / 100 + 1;

            foreach (var observer in _observers)
            {
                if (CurrentLevel > oldLevel)
                    observer.OnLevelUp(CurrentLevel, TotalXP);
            }
        }

        public void NotifyTaskCompleted(string taskTitle, int xp)
        {
            foreach (var observer in _observers)
                observer.OnTaskCompleted(taskTitle, xp);
        }

        public void NotifyAchievement(string achievement)
        {
            foreach (var observer in _observers)
                observer.OnAchievement(achievement);
        }
    }

    public class ConsoleLevelObserver : ILevelObserver
    {
        public void OnLevelUp(int newLevel, int totalXP)
        {
            System.Diagnostics.Debug.WriteLine($"LEVEL UP! Nivel {newLevel} atins!");
        }

        public void OnAchievement(string achievementName)
        {
            System.Diagnostics.Debug.WriteLine($"ACHIEVEMENT: {achievementName}");
        }

        public void OnTaskCompleted(string taskTitle, int xpGained)
        {
            System.Diagnostics.Debug.WriteLine($"Task '{taskTitle}' finalizat! +{xpGained} XP");
        }
    }

    public class AchievementObserver : ILevelObserver
    {
        private List<string> _unlockedAchievements = new List<string>();
        private int _completedTasks = 0;

        public void OnLevelUp(int newLevel, int totalXP)
        {
            if (newLevel == 5 && !_unlockedAchievements.Contains("level5"))
            {
                _unlockedAchievements.Add("level5");
                System.Diagnostics.Debug.WriteLine("Achievement deblocat: Nivel 5!");
            }
            if (newLevel == 10 && !_unlockedAchievements.Contains("level10"))
            {
                _unlockedAchievements.Add("level10");
                System.Diagnostics.Debug.WriteLine("Achievement deblocat: Nivel 10!");
            }
        }

        public void OnAchievement(string achievementName)
        {
            if (!_unlockedAchievements.Contains(achievementName))
            {
                _unlockedAchievements.Add(achievementName);
                System.Diagnostics.Debug.WriteLine($" Nou achievement: {achievementName}");
            }
        }

        public void OnTaskCompleted(string taskTitle, int xpGained)
        {
            _completedTasks++;
            if (_completedTasks == 10 && !_unlockedAchievements.Contains("10tasks"))
            {
                _unlockedAchievements.Add("10tasks");
                System.Diagnostics.Debug.WriteLine("Achievement deblocat: 10 taskuri finalizate!");
            }
            if (_completedTasks == 50 && !_unlockedAchievements.Contains("50tasks"))
            {
                _unlockedAchievements.Add("50tasks");
                System.Diagnostics.Debug.WriteLine("Achievement deblocat: 50 taskuri finalizate!");
            }
        }

        public List<string> GetUnlockedAchievements() {return  _unlockedAchievements;}
    }

    public class NotificationLoggerObserver : ILevelObserver
    {
        private List<Notification> _notifications = new List<Notification>();

        public void OnLevelUp(int newLevel, int totalXP)
        {
            _notifications.Add(new Notification
            {
                Type = "levelup",
                Message = $"Felicitări! Ai atins nivelul {newLevel}!",
                Timestamp = DateTime.Now
            });
        }

        public void OnAchievement(string achievementName)
        {
            _notifications.Add(new Notification
            {
                Type = "achievement",
                Message = $"Achievement deblocat: {achievementName}",
                Timestamp = DateTime.Now
            });
        }

        public void OnTaskCompleted(string taskTitle, int xpGained)
        {
            _notifications.Add(new Notification
            {
                Type = "task",
                Message = $"Task '{taskTitle}' finalizat! +{xpGained} XP",
                Timestamp = DateTime.Now
            });
        }

        public List<Notification> GetNotifications() {return _notifications;}
    }

  public class Notification
{
    public string Type { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Notification()
    {
        Type = string.Empty;
        Message = string.Empty;
    }
}
}