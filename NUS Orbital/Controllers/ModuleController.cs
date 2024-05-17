using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using MySql.Data.MySqlClient;

namespace NUS_Orbital.Controllers
{
    public class ModuleController : Controller
    {
        private ModuleDAL moduleContext = new ModuleDAL();
        private StudentDAL studentContext = new StudentDAL();

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("authenticated") == "true") {
                List<Module> moduleList = moduleContext.GetAllModules();
                return View(moduleList);
            }
            TempData["ModuleLogin"] = "Login to view more info about modules";
            return RedirectToAction("Login", "Home");
        }

        public ActionResult View(string ModuleCode)
        {
            Module module = moduleContext.getModuleDetails(ModuleCode);
            List<Post> postList = moduleContext.GetAllPosts(module);
            return View(new ModulePost(module, postList));
        }
    }
}
