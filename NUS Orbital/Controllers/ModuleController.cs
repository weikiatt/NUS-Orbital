using Microsoft.AspNetCore.Mvc;

namespace NUS_Orbital.Controllers
{
    public class ModuleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
