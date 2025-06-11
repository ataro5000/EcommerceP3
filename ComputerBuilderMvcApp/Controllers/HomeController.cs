using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using ComputerBuilderMvcApp.ViewModels; 
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
            var featuredComponents = allComponents.OrderBy(c => random.Next()).Take(4).ToList();
            
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
        public IActionResult FeedbackThanks()
        {

            return View();
        }
        
        [HttpPost]
        public IActionResult SubmitFeedback(FeedbackViewModel model)
        {
            if(ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("FeedbackThanks");
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