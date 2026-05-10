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
        public string GetExtension()
        {
                return ".json";
        }

        public string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        public void WriteFile(string path, string content)
        {
            System.IO.File.WriteAllText(path, content);
        }
    }

    public class TXTFileAdapter : IFileAdapter
    {
        public string GetExtension()  
        {
                return ".txt";
        }
        public string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        public void WriteFile(string path, string content)
        {
            System.IO.File.WriteAllText(path, content);
        }
    }

    public class CSVFileAdapter : IFileAdapter
    {
        public string GetExtension()
        {
                return ".csv";
        }

        public string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        public void WriteFile(string path, string content)
        {
            System.IO.File.WriteAllText(path, content);
        }
    }
}
