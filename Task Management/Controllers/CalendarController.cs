using Microsoft.AspNetCore.Mvc;

namespace Task_Management.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            var isPremium = HttpContext.Session.GetString("IsPremium") == "true";
            ViewBag.IsPremium = isPremium;
            return View();
        }
    }
}