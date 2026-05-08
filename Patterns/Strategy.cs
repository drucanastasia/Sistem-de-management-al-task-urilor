using Task_Management.Models;

namespace Task_Management.Patterns
{
    public interface IExportStrategy
    {
        string Export(User user);
        string FileExtension { get; }
        string ContentType { get; }
    }

    public class JSONExportStrategy : IExportStrategy
    {
        public string FileExtension => ".json";
        public string ContentType => "application/json";

        public string Export(User user)
        {
            return System.Text.Json.JsonSerializer.Serialize(user, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }

    public class TXTExportStrategy : IExportStrategy
    {
        public string FileExtension => ".txt";
        public string ContentType => "text/plain";

        public string Export(User user)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"TaskManagement - {user.Username}");
            sb.AppendLine($"Nivel: {user.Level} | XP: {user.XP}");
            sb.AppendLine();
            sb.AppendLine("TASKURI:");
            foreach (var task in user.Tasks)
            {
                sb.AppendLine($"- [{task.Category}] {task.Title} (+{task.XP} XP)");
            }
            sb.AppendLine();
            sb.AppendLine("EVENIMENTE:");
            foreach (var ev in user.Events)
            {
                sb.AppendLine($"- [{ev.Color}] {ev.Title} ({ev.Date:dd.MM})");
            }
            return sb.ToString();
        }
    }

    public class CSVExportStrategy : IExportStrategy
    {
        public string FileExtension => ".csv";
        public string ContentType => "text/csv";

        public string Export(User user)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Type,Title,Category,Status,Date");

            foreach (var task in user.Tasks)
            {
                sb.AppendLine($"Task,\"{task.Title}\",{task.Category},{task.Status},{task.CreatedAt:yyyy-MM-dd}");
            }

            foreach (var ev in user.Events)
            {
                sb.AppendLine($"Event,\"{ev.Title}\",{ev.Color},,{ev.Date:yyyy-MM-dd}");
            }

            return sb.ToString();
        }
    }

}
