namespace Task_Management.Interface
{
    // ✅ PATTERN: Composite - Interfața comună pentru task individual și grup de taskuri
    public interface ITaskComponent
    {
        string GetTitle();
        int GetXP();
        bool IsCompleted();
        void Complete();
    }
}
