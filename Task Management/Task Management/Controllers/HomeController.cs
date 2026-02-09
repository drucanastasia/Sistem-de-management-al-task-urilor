using Microsoft.AspNetCore.Mvc;

namespace Task_Management.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
