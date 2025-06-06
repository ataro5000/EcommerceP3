using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ComputerBuilderMvcApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Feedback()
        {
            return View();
        }
    }
}