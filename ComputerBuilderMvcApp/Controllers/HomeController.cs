using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models; // If needed for featured items, etc.
using ComputerBuilderMvcApp.ViewModels; // If using ViewModels for the home page
using System.Diagnostics; // For ErrorViewModel
using System.Linq; // If fetching data
using Newtonsoft.Json; // If loading from JSON
using System.IO; // For Path

namespace ComputerBuilderMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _computersDataFilePath;

        public HomeController()
        {
            _computersDataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "computers.json");
        }

        public IActionResult Index()
        {
            // Optionally, load some featured computers to pass to the view
            var featuredComputers = new List<ComputerViewModel>();
            if (System.IO.File.Exists(_computersDataFilePath))
            {
                var json = System.IO.File.ReadAllText(_computersDataFilePath);
                var allComputers = JsonConvert.DeserializeObject<List<Computer>>(json);
                if (allComputers != null)
                {
                    // Example: Take the first 3 as featured
                    featuredComputers = allComputers.Take(3).Select(c => new ComputerViewModel {
                        Id = c.ID,
                        Name = c.Name ?? "N/A",
                        PriceCents = c.PriceCents / 100,
                        // Map other properties if your Home/Index.cshtml expects them
                    }).ToList();
                }
            }
            return View(featuredComputers); // Pass data to your Home/Index.cshtml
        }

        public IActionResult Privacy()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();//(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}