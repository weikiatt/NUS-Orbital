using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using System.Globalization;

namespace NUS_Orbital.Controllers
{
    public class ChatController : Controller
    {
        private ModuleDAL moduleContext = new ModuleDAL();
        private StudentDAL studentContext = new StudentDAL();
        private ChatDAL chatContext = new ChatDAL();
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                Student currStud = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"));
                return View(new ChatList(currStud, chatContext.GetAllChatsForUser(currStud)));
            }
            TempData["Login"] = "Login to chat";
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public JsonResult SendMessage(int currStudId, string otherStudId, string message, string currentTime)
        {
            return Json(new { success = true });
            /*
            TempData["TEST"] = "TEST";
            string dateFormat = "dd/MM/yyyy hh:mm:ss tt";
            if (message == "")
            {
                return Json(new { success = false });
            }
            try
            {
                chatContext.SendMessage(Convert.ToInt32(currStudId),Convert.ToInt32(otherStudId), message, DateTime.ParseExact(currentTime, dateFormat, CultureInfo.InvariantCulture));
            } catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });*/
        }


    }
}
