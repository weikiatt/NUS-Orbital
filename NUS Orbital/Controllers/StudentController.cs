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
        public ActionResult UploadPhoto(int id)
        {
            string email = HttpContext.Session.GetString("Email");

            Student student = studentContext.GetStudentDetailsWithEmail(email);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(Student student)
        {
            if (student.fileToUpload != null && student.fileToUpload.Length > 0)
            {
                try
                {
                    // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(
                    student.fileToUpload.FileName);
                    // Rename the uploaded file with the student’s name.
                    string uploadedFile = student.name + fileExt;
                    // Get the complete path to the images folder in server
                    string savePath = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot\\images\\StudentPhotos", uploadedFile);
                    // Upload the file to server
                    using (var fileSteam = new FileStream(
                     savePath, FileMode.Create))
                    {
                        await student.fileToUpload.CopyToAsync(fileSteam);
                    }
                    student.photo = uploadedFile;
                    TempData["Message"] = "File uploaded successfully.";
                }
                catch (IOException)
                {
                    //File IO error, could be due to access rights denied
                    TempData["Message"] = "File uploading fail!";
                }
                catch (Exception ex) //Other type of error
                {
                    TempData["Message"] = ex.Message;
                }
            }
            string email = HttpContext.Session.GetString("Email");
            Student temp = studentContext.GetStudentDetailsWithEmail(email);
            student.studentId = temp.studentId; 

            studentContext.UpdatePhoto(student);
            return View(student);
        }

        [HttpGet]
        public ActionResult Account()
        {
            Student student = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"));
            return View(student);
        }
    }
}
