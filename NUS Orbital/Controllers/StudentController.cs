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
            student.StudentId = oldStudent.StudentId;
            if (student.Course == null)
            {
                student.Course = "";
            }
            if (student.Description == null)
            {
                student.Description = "";
            }
            TempData["Message"] = "Changes saved successfully.";
            studentContext.UpdateStudent(student);
            if (student.FileToUpload != null && student.FileToUpload.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await student.FileToUpload.CopyToAsync(memoryStream);
                        byte[] fileData = memoryStream.ToArray();
                        studentContext.UpdatePhoto2(student, fileData);
                        student = studentContext.GetStudentDetailsWithID(student.StudentId);
                    }
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
                student.Photo = oldStudent.Photo;

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
                Student currStud = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"));
                Student studToView = studentContext.GetStudentDetailsWithEmail(email);
                return View(new StudentView(currStud, studToView));
            }
            TempData["Login"] = "Login to view other users account info";
            return RedirectToAction("Login", "Home");
        }
    }
}
