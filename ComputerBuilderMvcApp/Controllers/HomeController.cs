// This file defines the HomeController class, which handles requests for the main pages of the application,
// such as the home page, contact page, and feedback submission.
using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.ViewModels;
using System.Diagnostics;

namespace ComputerBuilderMvcApp.Controllers
{
    public class HomeController : Controller
    {
        // Constructor for the HomeController.
        public HomeController()
        {
        }

        // Displays the home page.
        // It loads all components, selects a few random ones as featured components, and passes them to the view.
        public IActionResult Index(List<string> categories)
        {
            var allComponents = ComponentsController.LoadComponents(categories);
            var random = new Random();
            var featuredComponents = allComponents.OrderBy(c => random.Next()).Take(4).ToList();
            return View(featuredComponents);
        }

        // Displays the contact page.
        public IActionResult Contact()
        {
            return View();
        }

        // Displays the feedback submission page.
        public IActionResult Feedback()
        {
            return View();
        }

        // Displays the feedback thank you page.
        public IActionResult FeedbackThanks()
        {
            return View();
        }

        // Handles the submission of feedback.
        // If the model state is valid, it sets a success message and redirects to the feedback thank you page.
        // Otherwise, it returns to the feedback page with the current model to display validation errors.
        [HttpPost]
        public IActionResult SubmitFeedback(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("FeedbackThanks");
            }
            return View("Feedback", model);
        }

        // Displays the error page.
        // This action is configured to not cache the response.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}