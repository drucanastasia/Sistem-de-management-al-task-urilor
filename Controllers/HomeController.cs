using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Calendar()
        {
            var viewModel = new CalendarViewModel
            {
                CurrentYear = DateTime.Now.Year,
                IsPremium = false // Schimbă în true după plată
            };

            return View(viewModel);
        }

        public IActionResult Profil()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetMonths(int year)
        {
            var months = new List<string>
            {
                "Ianuarie", "Februarie", "Martie", "Aprilie", "Mai", "Iunie",
                "Iulie", "August", "Septembrie", "Octombrie", "Noiembrie", "Decembrie"
            };

            var monthData = months.Select((name, index) => new
            {
                MonthNumber = index + 1,
                MonthName = name,
                DaysInMonth = DateTime.DaysInMonth(year, index + 1)
            }).ToList();

            return Json(monthData);
        }

        [HttpGet]
        public IActionResult GetDaysInMonth(int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var days = new List<object>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                days.Add(new
                {
                    Day = day,
                    Date = date.ToString("yyyy-MM-dd"),
                    DayOfWeek = date.DayOfWeek.ToString(),
                    IsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday
                });
            }

            return Json(days);
        }

        [HttpPost]
        public IActionResult SaveDayNote(int year, int month, int day, string note)
        {
            // Aici vei salva în baza de date mai târziu
            return Json(new { success = true, message = "Notiță salvată!" });
        }

        [HttpPost]
        public IActionResult UpgradeToPremium(string plan)
        {
            // Aici actualizezi user-ul în baza de date
            return Json(new { success = true, message = "Felicitări! Ai acces la calendar!" });
        }
        public IActionResult Tasks(string category)
        {
            // Validare categorie
            var validCategories = new[] { "university", "work", "travel", "personal" };
            if (!validCategories.Contains(category))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Category = category;
            return View();
        }
    }

}