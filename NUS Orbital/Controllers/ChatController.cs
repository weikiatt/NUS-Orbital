using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using System.Globalization;


using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1;
using Mysqlx.Crud;

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
        public JsonResult SendMessage(int currStudId, int otherStudId, string msg)
        {
            /*
            if (msg == "")
            {
                return Json(new { success = false });
            }*/
            DateTime currTime = DateTime.Now;
            chatContext.SendMessage(currStudId, otherStudId, msg, currTime);
            if (!chatContext.DoesChatExist(otherStudId, currStudId))
            {
                chatContext.AddChat(otherStudId, currStudId);
            }
            return Json(new { success = true });
        }

        public ActionResult AddChatFromView(IFormCollection formData)
        {
            int currStudId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).StudentId;
            int otherStudId = Convert.ToInt32(formData["otherStudId"]);
            if (!chatContext.DoesChatExist(currStudId, otherStudId))
            {
                chatContext.AddChat(currStudId, otherStudId);
            }
            return RedirectToAction("Index", "Chat");
        }




    }
}
