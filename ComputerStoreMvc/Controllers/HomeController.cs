using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ComputerStoreMvc.Models;

namespace ComputerStoreMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var computers = GetComputers();
            return View(computers);
        }

        private List<Computer> GetComputers()
        {
            var jsonData = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "computers.json"));
            return JsonConvert.DeserializeObject<List<Computer>>(jsonData);
        }
    }
}