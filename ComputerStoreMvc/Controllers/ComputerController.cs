using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComputerStoreMvc.Models;

namespace ComputerStoreMvc.Controllers
{
    public class ComputerController : Controller
    {
        private readonly string _computersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "computers.json");

        public IActionResult Index()
        {
            var computers = GetComputers();
            return View(computers);
        }

        public IActionResult Detail(int id)
        {
            var computers = GetComputers();
            var computer = computers.FirstOrDefault(c => c.ID == id);
            if (computer == null)
            {
                return NotFound();
            }
            return View(computer);
        }

        private List<Computer> GetComputers()
        {
            var jsonData = System.IO.File.ReadAllText(_computersFilePath);
            return JsonConvert.DeserializeObject<List<Computer>>(jsonData);
        }
    }
}