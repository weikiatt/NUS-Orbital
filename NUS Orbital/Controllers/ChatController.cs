﻿using Microsoft.AspNetCore.Mvc;

namespace NUS_Orbital.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
