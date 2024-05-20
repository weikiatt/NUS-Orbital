using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace NUS_Orbital.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private StudentDAL studentContext = new StudentDAL();


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            /*
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
			if (studentContext.doesLoginCredentialExist(email, password))
			{
                HttpContext.Session.SetString("authenticated", "true");
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("name", studentContext.GetName(email));
                return View("Index");
            }
			TempData["InvalidLogin"] = "Invalid login credentials!";
			return View("Login", "Home");   
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Student student)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("authenticated", "true");
                HttpContext.Session.SetString("Email", student.email);
                HttpContext.Session.SetString("name", student.name);
                studentContext.Add(student);
                return View("Index");
            }
            return View(student);

        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
