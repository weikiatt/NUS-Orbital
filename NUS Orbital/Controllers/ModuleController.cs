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

        [HttpGet]
        public IActionResult Index(string query)
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                List<Module> moduleList = moduleContext.GetAllModules();
                if (query == null)
                {
                    return View(moduleList);
                }
                query = query.ToLower();
                List<Module> filteredModuleList = new List<Module>();
                foreach (Module module in  moduleList)
                {
                    if(module.moduleCode.ToLower().Contains(query)  || module.moduleName.ToLower().Contains(query))
                    {
                        filteredModuleList.Add(module);
                    }
                }
                return View(filteredModuleList);
            }
            TempData["Login"] = "Login to view more info about modules";
            return RedirectToAction("Login", "Home");
        }

        public ActionResult View(string ModuleCode)
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                Module module = moduleContext.getModuleDetails(ModuleCode);
                List<Post> postList = moduleContext.GetAllPosts(module);
                return View(new ModulePost(module, postList, studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"))));
            }
            TempData["Login"] = "Login to view more info about modules";
            return RedirectToAction("Login", "Home");
        }

        
        public ActionResult Post(IFormCollection formData)
        {
            string description = formData["description"].ToString();
            string moduleCode = formData["moduleCode"].ToString();
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).studentId;
            if (description != null)
            {
                moduleContext.AddPost(moduleCode, description, studentId);
            }
            return RedirectToAction("View", "Module", new {ModuleCode = moduleCode});
        }

        public ActionResult Comment(IFormCollection formData)
        {
            string description = formData["description"].ToString();
            string moduleCode = formData["moduleCode"].ToString();
            int postId = Convert.ToInt32(formData["postId"]);
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).studentId;
            if (description != null)
            {
                moduleContext.AddCommentToPost(description, postId, studentId);
            }
            return RedirectToAction("View", "Module", new { ModuleCode = moduleCode });
        }

    }
}
