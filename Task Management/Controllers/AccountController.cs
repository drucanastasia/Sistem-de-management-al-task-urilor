using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{

    public IActionResult Login() => View();

    // Procesare login
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        //  logica de autentificare
    

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {

            return RedirectToAction("Index", "Home");
        }

        return View(); 
    }

    // Pagina de înregistrare
    public IActionResult Register() => View();

    // Procesare înregistrare
    [HttpPost]
    public IActionResult Register(string username, string email, string password)
    {
        //  logica reală de înregistrare 
    

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }
}