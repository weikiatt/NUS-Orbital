using Microsoft.AspNetCore.Mvc;

namespace NUS_Orbital.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View("Login");
        }

     
        [HttpGet]
        public ActionResult Register()
        {
            return RedirectToAction("Create", "Student");
        }
    }
}
