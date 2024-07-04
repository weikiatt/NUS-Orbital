using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using System.Diagnostics;

namespace NUS_Orbital.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private StudentDAL studentContext = new StudentDAL();
        private ModuleDAL moduleContext = new ModuleDAL();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            /* test34
            HttpContext.Session.SetString("authenticated", "true");
            HttpContext.Session.SetString("name", "temp name");*/
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(IFormCollection formData)
        {
            string email = formData["email"].ToString().ToLower();
            string password = formData["password"].ToString();
            if (studentContext.DoesLoginCredentialExist(email, password))
            {
                HttpContext.Session.SetString("authenticated", "true");
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("name", studentContext.GetName(email));
                return View("Index");
            }
            TempData["Login"] = "Invalid login credentials!";
            return View("Login", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Student student)
        {
            bool register = true;
            if(studentContext.DoesEmailExist(student.Email))
            {
                TempData["EmailValidation"] = "Email already exists!";
                register = false;
            }
            if (student.Password.Length < 8)
            {
                TempData["PasswordLength"] = "Minimum password length is 8";
                register = false;
            }
            if (!student.Password.Any(char.IsUpper))
            {
                TempData["PasswordUpperCase"] = "Password should contain one uppercase";
                register = false;
            }
            if (!student.Password.Any(char.IsLower))
            {
                TempData["PasswordLowerCase"] = "Password should contain one lowercase";
                register = false;
            }
            if (!register)
            {
                return View(student);
            }

            HttpContext.Session.SetString("authenticated", "true");
            HttpContext.Session.SetString("Email", student.Email);
            HttpContext.Session.SetString("name", student.Name);

            student.ProfilePicture = GetImageAsByteArray(Url.Content("wwwroot/images/StudentPhotos/user.png"));
            studentContext.Add(student);
            return View("Index");
        }

        public byte[] GetImageAsByteArray(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        /*
        public IActionResult FileUpload()
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                Student currStud = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"));
                Module module = moduleContext.GetModuleDetails("CS1010");
                List<Post> postList = moduleContext.GetAllPosts(module, currStud);
               

                return View(new ModulePost(module, postList, currStud));
            }
            TempData["Login"] = "Login to view more info about modules";
            return RedirectToAction("Login", "Home");
        }*/

        
    }
}
