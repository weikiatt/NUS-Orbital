using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;

namespace NUS_Orbital.Controllers
{
    public class StudentController : Controller
    {
        private ModuleDAL moduleContext = new ModuleDAL();
        private StudentDAL studentContext = new StudentDAL();
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
            return View();
        }

        [HttpGet]
        public ActionResult Account()
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                Student student = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"));
                return View(student);
            }
            TempData["Login"] = "Login to view account info";
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Account(Student student)
        {
            Student oldStudent = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"));
            student.studentId = oldStudent.studentId;
            studentContext.UpdateStudent(student);
            if (student.fileToUpload != null && student.fileToUpload.Length > 0)
            {
                try
                {
                    string fileExt = Path.GetExtension(
                    student.fileToUpload.FileName);
                    string uploadedFile = student.name + fileExt;
                    string savePath = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot\\images\\StudentPhotos", uploadedFile);
                    using (var fileSteam = new FileStream(
                     savePath, FileMode.Create))
                    {
                        await student.fileToUpload.CopyToAsync(fileSteam);
                    }
                    student.photo = uploadedFile;
                    TempData["Message"] = "File uploaded successfully.";
                    studentContext.UpdatePhoto(student);
                }
                catch (IOException)
                {
                    TempData["Message"] = "File uploading fail!";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            } else
            {
                student.photo = oldStudent.photo;
            }
            return View(student);
        }

        /*
        [HttpPost]
        public ActionResult View(IFormCollection formData)
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                Student student = studentContext.GetStudentDetailsWithEmail(formData["email"].ToString().ToLower());
                TempData["email"] = formData["email"].ToString().ToLower();
                return View(student);
            }
            TempData["Login"] = "Login to view other users account info";
            return RedirectToAction("Login", "Home");
        }*/

        [HttpGet]
        public ActionResult ViewAccount(string email)
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                Student student = studentContext.GetStudentDetailsWithEmail(email);
                return View(student);
            }
            TempData["Login"] = "Login to view other users account info";
            return RedirectToAction("Login", "Home");
        }
    }
}
