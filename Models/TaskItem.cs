

using Task_Management.Interface;
using Task_Management.Enum;

namespace Task_Management.Models
{
 
   public class TaskItem : ITaskComponent
{
    
    private string id = Guid.NewGuid().ToString();
    private string title = string.Empty;
    private string category = string.Empty;
    private string description = string.Empty;
    private string status = "todo";
    private int xp = 10;
    private DateTime createdAt = DateTime.Now;

  
    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    public string Category
    {
        get { return category; }
        set { category = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public string Status
    {
        get { return status; }
        set { status = value; }
    }

    public int XP
    {
        get { return xp; }
        set { xp = value; }
    }

    public DateTime CreatedAt
    {
        get { return createdAt; }
        set { createdAt = value; }
    }

    public string GetTitle()
    {
        return Title;
    }

    public int GetXP()
    {
        return XP;
    }

    public bool IsCompleted()
    {
        return Status == "done";
    }

    public void Complete()
    {
        Status = "done";
    }
}
}