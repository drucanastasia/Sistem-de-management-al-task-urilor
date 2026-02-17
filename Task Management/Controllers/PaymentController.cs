using Microsoft.AspNetCore.Mvc;

namespace Task_Management.Controllers
{
    public class PaymentController : Controller
    {
        public PaymentController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessPayment()
        {
         
           
            return RedirectToAction("Index", "Payment");
        }
    }
}