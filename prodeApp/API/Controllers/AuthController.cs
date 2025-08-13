using Microsoft.AspNetCore.Mvc;

namespace prodeApp.API.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
