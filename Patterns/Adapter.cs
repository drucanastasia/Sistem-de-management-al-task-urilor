namespace Task_Management.Patterns
{
    public interface IFileAdapter
    {
        string ReadFile(string path);
        void WriteFile(string path, string content);
        string GetExtension();
    }

    public class JSONFileAdapter : IFileAdapter
    {
        public string GetExtension() => ".json";

        public string ReadFile(string path) => System.IO.File.ReadAllText(path);

        public void WriteFile(string path, string content) => System.IO.File.WriteAllText(path, content);
    }

    public class TXTFileAdapter : IFileAdapter
    {
        public string GetExtension() => ".txt";

        public string ReadFile(string path) => System.IO.File.ReadAllText(path);

        public void WriteFile(string path, string content) => System.IO.File.WriteAllText(path, content);
    }

    public class CSVFileAdapter : IFileAdapter
    {
        public string GetExtension() => ".csv";

        public string ReadFile(string path) => System.IO.File.ReadAllText(path);

        public void WriteFile(string path, string content) => System.IO.File.WriteAllText(path, content);
    }
}
