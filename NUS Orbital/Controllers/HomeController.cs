using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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
        private AdminDAL adminContext = new AdminDAL();
        private HomeDAL homeContext = new HomeDAL();
        private readonly EmailService _emailService = new EmailService();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendEmail(string toEmail, string subject, string body)
        {
            await _emailService.SendEmailAsync(toEmail, subject, body);
            return RedirectToAction("Index");
        }


        public IActionResult Index()
        {
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
                if (!homeContext.IsStudentVerified(email))
                {
                    TempData["VerificationCode"] = "A verification code has been sent to " + email + " before, please verify your email first, remember to check your spam or junk folders if you cannot find the email.";
                    return RedirectToAction("Verification", new { StudentId = studentContext.GetStudentDetailsWithEmail(email).StudentId });
                }
                HttpContext.Session.SetString("authenticated", "true");
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("name", studentContext.GetName(email));
                HttpContext.Session.SetString("role", "user");
                return View("Index");
            }
            TempData["Login"] = "Invalid login credentials!";
            return View("Login", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Student student, IFormCollection formData)
        {
            
            student.ProfilePicture = GetImageAsByteArray(Url.Content("wwwroot/images/StudentPhotos/user.png"));
            studentContext.Add(student);
            student = studentContext.GetStudentDetailsWithEmail(student.Email);
            string verificationCode = VerificationCodeGenerator.GenerateCode();
            _emailService.SendVerificationCodeAsync(student.Email, verificationCode);
            homeContext.AddVerification(student.StudentId, verificationCode, DateTime.Now);
            TempData["VerificationCode"] = "A verification code has been sent to " + student.Email + ", please check your spam or junk folders if you cannot find the email.";
            return RedirectToAction("Verification", new { StudentId = student.StudentId });
        }


        [HttpGet]
        public ActionResult Verification(int studentId)
        {
            if (TempData["VerificationCode"] == null)
            {
                return RedirectToAction("Login");
            }
            var student = studentContext.GetStudentDetailsWithID(studentId);
            return View(student);
        }

        [HttpPost]
        public ActionResult Verification(Student student, IFormCollection formData)
        {
            string verificationCode = formData["input1"].ToString() + formData["input2"].ToString() + formData["input3"].ToString() 
                + formData["input4"].ToString() + formData["input5"].ToString() + formData["input6"].ToString();
            
            homeContext.Expire();
            if (homeContext.VerifyCode(student.StudentId, verificationCode))
            {
                homeContext.VerifyStudent(student.StudentId);
                homeContext.ExpireCode(student.StudentId);
                student = studentContext.GetStudentDetailsWithID(student.StudentId);
                HttpContext.Session.SetString("authenticated", "true");
                HttpContext.Session.SetString("Email", student.Email);
                HttpContext.Session.SetString("name", student.Name);
                HttpContext.Session.SetString("role", "user");
                return RedirectToAction("Index");
            }
            TempData["EXPIRED"] = "Verification code is wrong or has expired";
            return View(student);
        }

        [HttpGet]
        public ActionResult ResendVerification(int studentId)
        {
            Student student = studentContext.GetStudentDetailsWithID(studentId);
            homeContext.ExpireCode(student.StudentId);
            string verificationCode = VerificationCodeGenerator.GenerateCode();
            _emailService.SendVerificationCodeAsync(student.Email, verificationCode);
            homeContext.AddVerification(student.StudentId, verificationCode, DateTime.Now);
            TempData["VerificationCode"] = "A verification code has been resent to " + student.Email + ", please check your spam or junk folders if you cannot find the email.";
            return RedirectToAction("Verification", new { StudentId = student.StudentId });
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

        public ActionResult AdminLogin()
        {
            return View("AdminLogin");
        }

        [HttpPost]
        public IActionResult AdminLogin(IFormCollection formData)
        {
            string name = formData["name"].ToString().ToLower();
            string password = formData["password"].ToString();
            if (adminContext.DoesLoginCredentialExist(name, password))
            {
                HttpContext.Session.SetString("authenticated", "true");
                HttpContext.Session.SetString("Email", "admin");
                HttpContext.Session.SetString("name", "admin");
                HttpContext.Session.SetString("role", "admin");
                return View("Index");
            }
            TempData["Login"] = "Invalid login credentials!";
            return RedirectToAction("AdminLogin", "Home");
        }
    }
}
