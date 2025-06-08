using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using ComputerBuilderMvcApp.ViewModels; // If you have a specific ViewModel for the home page
using Newtonsoft.Json;
using System.Diagnostics;

namespace ComputerBuilderMvcApp.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }
        public IActionResult Index(List<string> categories)
        {
            var allComponents = ComponentsController.LoadComponents(categories);
            var random = new Random();
            var featuredComponents = allComponents.OrderBy(c => random.Next()).Take(6).ToList();
            
            return View(featuredComponents);
        }
      

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Feedback()
        {

            return View();
        }
        
        [HttpPost]
        public IActionResult SubmitFeedback(FeedbackViewModel model)
        {
            if(ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("Index");
            }
            return View("Feedback", model);
        }
      
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
}