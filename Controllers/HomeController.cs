using Microsoft.AspNetCore.Mvc;
using Task_Management.Models;
using Task_Management.Patterns;


namespace Task_Management.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Dictionary<string, TaskFacade> _facades = new();
        private static readonly Dictionary<string, User>       _users   = new();
        private static readonly Dictionary<string, List<Task_Management.Patterns.CalendarEvent>> _events = new();
        private static readonly Dictionary<string, List<Note>> _notes   = new();

        private TaskFacade GetFacade(string username)
        {
            if (!_facades.ContainsKey(username))
                _facades[username] = new TaskFacade();
            return _facades[username];
        }

        private string? CurrentUser => HttpContext.Session.GetString("username");
        public IActionResult Index() => View();

        [HttpPost] public IActionResult Login([FromBody] LoginDto dto)
        {
            if (!_users.TryGetValue(dto.Username, out var user) || user.Password != dto.Password)
                return BadRequest(new { error = "Utilizator sau parolă greșite." });
            HttpContext.Session.SetString("username", dto.Username);
            return Ok(new { username = user.Username, firstName = user.FirstName, lastName = user.LastName });
        }

        [HttpPost] public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (_users.ContainsKey(dto.Username))
                return BadRequest(new { error = "Utilizatorul există deja." });
            var user = new User { Username = dto.Username, Password = dto.Password, FirstName = dto.FirstName, LastName = dto.LastName };
            _users[dto.Username] = user;

            // Demo tasks via Facade (Abstract Factory)
            var facade = GetFacade(dto.Username);
            facade.AddTaskWithCommand("Bun venit în TaskManagement!", "freetime", "Acesta este primul tău task.");
            facade.AddTaskWithCommand("Explorează toate funcțiile", "work", "Calendar, Notițe, Export, Șabloane...");

            HttpContext.Session.SetString("username", dto.Username);
            return Ok(new { username = user.Username, firstName = user.FirstName, lastName = user.LastName });
        }

        [HttpPost] public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return Ok();
        }

        [HttpGet] public IActionResult GetTasks()
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            return Ok(new { tasks = facade.GetAllTasks().Select(TaskDto), level = facade.GetCurrentLevel(), xp = facade.GetTotalXP(), canUndo = facade.CanUndo });
        }

       
        [HttpPost] public IActionResult AddTask([FromBody] AddTaskDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            var task = facade.AddTaskWithCommand(dto.Title, dto.Category, dto.Description ?? "");
            return Ok(new { task = TaskDto(task), level = facade.GetCurrentLevel(), xp = facade.GetTotalXP(), canUndo = facade.CanUndo });
        }

     
        [HttpPost] public IActionResult AddTaskBuilder([FromBody] AddTaskBuilderDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            var task = facade.CreateTaskWithBuilder(dto.Title, dto.Category, dto.Description ?? "", dto.XP);
            return Ok(new { task = TaskDto(task), level = facade.GetCurrentLevel(), xp = facade.GetTotalXP(), canUndo = facade.CanUndo });
        }

    
        [HttpPost] public IActionResult AddTaskFromTemplate([FromBody] TemplateKeyDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            var task = facade.CreateTaskFromTemplate(dto.Key);
            if (task == null) return BadRequest(new { error = "Template inexistent." });
            return Ok(new { task = TaskDto(task), level = facade.GetCurrentLevel(), xp = facade.GetTotalXP(), canUndo = facade.CanUndo });
        }

        [HttpDelete] public IActionResult DeleteTask(string id)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            if (!facade.DeleteTaskWithCommand(id)) return NotFound();
            return Ok(new { canUndo = facade.CanUndo });
        }

       
        [HttpPost] public IActionResult CompleteTask([FromBody] IdDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            if (!facade.CompleteTaskWithXP(dto.Id)) return NotFound();
            return Ok(new { level = facade.GetCurrentLevel(), xp = facade.GetTotalXP(), canUndo = facade.CanUndo });
        }

       
        [HttpPost] public IActionResult AdvanceState([FromBody] IdDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            if (!facade.AdvanceTaskState(dto.Id)) return NotFound();
            var status = facade.GetTaskState(dto.Id);
            int level = facade.GetCurrentLevel(), xp = facade.GetTotalXP();
            if (status == "done") { facade.CompleteTaskWithXP(dto.Id); level = facade.GetCurrentLevel(); xp = facade.GetTotalXP(); }
            return Ok(new { status = facade.GetTaskState(dto.Id) ?? status, level, xp });
        }

        
        [HttpPost] public IActionResult RevertState([FromBody] IdDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            if (!facade.RevertTaskState(dto.Id)) return NotFound();
            return Ok(new { status = facade.GetTaskState(dto.Id) });
        }


        [HttpPost] public IActionResult SetStatus([FromBody] SetStatusDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            if (!facade.UpdateStatusWithCommand(dto.Id, dto.Status)) return NotFound();
            int level = facade.GetCurrentLevel(), xp = facade.GetTotalXP();
            if (dto.Status == "done") { facade.CompleteTaskWithXP(dto.Id); level = facade.GetCurrentLevel(); xp = facade.GetTotalXP(); }
            return Ok(new { level, xp, canUndo = facade.CanUndo });
        }

        
        [HttpPost] public IActionResult Undo()
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            if (!facade.UndoLastCommand()) return BadRequest(new { error = "Nimic de anulat." });
            return Ok(new { tasks = facade.GetAllTasks().Select(TaskDto), canUndo = facade.CanUndo, level = facade.GetCurrentLevel(), xp = facade.GetTotalXP() });
        }

        [HttpGet] public IActionResult GetEvents()
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            if (!_events.TryGetValue(u, out var list)) list = new();
            return Ok(list.Select(EventDto));
        }

        [HttpPost] public IActionResult AddEvent([FromBody] AddEventDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
         
            var ev = Factory_Method.CreateEvent(dto.Title, DateTime.Parse(dto.Date), dto.Color);
         
            var facade = GetFacade(u);
            var decorated = facade.DecorateEvent(ev, dto.Color);
            ev.Title = decorated.GetTitle();
            ev.Color = decorated.GetColor();
            if (!_events.ContainsKey(u)) _events[u] = new();
            _events[u].Add(ev);
            return Ok(EventDto(ev));
        }

        [HttpDelete] public IActionResult DeleteEvent(string id)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            if (!_events.TryGetValue(u, out var list)) return NotFound();
            var ev = list.FirstOrDefault(e => e.Id == id);
            if (ev == null) return NotFound();
            list.Remove(ev);
            return Ok();
        }
        [HttpGet] public IActionResult GetNotes()
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            if (!_notes.TryGetValue(u, out var list)) list = new();
            return Ok(list);
        }

        [HttpPost] public IActionResult AddNote([FromBody] NoteDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            if (!_notes.ContainsKey(u)) _notes[u] = new();
            var note = new Note { Id = Guid.NewGuid().ToString(), Content = dto.Content, CreatedAt = DateTime.Now };
            _notes[u].Add(note);
            return Ok(note);
        }

        [HttpDelete] public IActionResult DeleteNote(string id)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            if (!_notes.TryGetValue(u, out var list)) return NotFound();
            var note = list.FirstOrDefault(n => n.Id == id);
            if (note == null) return NotFound();
            list.Remove(note);
            return Ok();
        }

        [HttpPut] public IActionResult UpdateNote(string id, [FromBody] NoteDto dto)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            if (!_notes.TryGetValue(u, out var list)) return NotFound();
            var note = list.FirstOrDefault(n => n.Id == id);
            if (note == null) return NotFound();
            note.Content = dto.Content;
            return Ok(note);
        }

        [HttpGet] public IActionResult Export(string format)
        {
            var u = CurrentUser; if (u == null) return Unauthorized();
            var facade = GetFacade(u);
            facade.SetExportStrategy(format);
            if (!_users.TryGetValue(u, out var user)) return NotFound();
            user.Tasks = facade.GetAllTasks();
            user.Events = _events.TryGetValue(u, out var evs)
                ? evs.Select(e => new Task_Management.Models.CalendarEvent { Id = e.Id, Title = e.Title, Date = e.Date, Color = e.Color, CreatedAt = e.CreatedAt }).ToList()
                : new();
            var content = facade.ExportUserData(user) ?? "";
            var ext = facade.GetExportExtension();
            var ct  = format == "json" ? "application/json" : format == "csv" ? "text/csv" : "text/plain";
            return File(System.Text.Encoding.UTF8.GetBytes(content), ct, $"taskmanagement_export{ext}");
        }

        [HttpGet] public IActionResult GetTemplates() => Ok(new[]
        {
            new { key = "work_meeting",   name = "Meeting echipă",  category = "work",     desc = "Meeting săptămânal" },
            new { key = "travel_booking", name = "Rezervare bilet", category = "travel",   desc = "Rezervare transport/cazare" },
            new { key = "workout",        name = "Sală",            category = "freetime", desc = "30 minute antrenament" },
            new { key = "rest_break",     name = "Pauză",           category = "rest",     desc = "Pauză de 15 minute" }
        });

        private static object TaskDto(TaskItem t) => new
        { id = t.Id, title = t.Title, category = t.Category, description = t.Description, status = t.Status, xp = t.XP, createdAt = t.CreatedAt };

        private static object EventDto(Task_Management.Patterns.CalendarEvent e) => new
        { id = e.Id, title = e.Title, date = e.Date.ToString("yyyy-MM-dd"), color = e.Color, createdAt = e.CreatedAt };
    }

    // DTOs
    public record LoginDto(string Username, string Password);
    public record RegisterDto(string Username, string Password, string FirstName, string LastName);
    public record AddTaskDto(string Title, string Category, string? Description);
    public record AddTaskBuilderDto(string Title, string Category, string? Description, int XP);
    public record TemplateKeyDto(string Key);
    public record IdDto(string Id);
    public record SetStatusDto(string Id, string Status);
    public record AddEventDto(string Title, string Date, string Color);
    public record NoteDto(string Content);
}
