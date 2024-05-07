using Microsoft.AspNetCore.Mvc;

namespace NUS_Orbital.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
