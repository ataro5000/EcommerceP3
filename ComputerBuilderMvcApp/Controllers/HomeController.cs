using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using ComputerBuilderMvcApp.ViewModels; // If you have a specific ViewModel for the home page
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ComputerBuilderMvcApp.Controllers
{
    public class HomeController : Controller
    {
        // If you create a dedicated service for loading components, inject it here.
        // For now, we'll use a private helper method.

        public HomeController()
        {
            // Constructor
        }

        public IActionResult Index()
        {
            var allComponents = LoadAllSystemComponents();
            var random = new Random();
            var featuredComponents = allComponents.OrderBy(c => random.Next()).Take(6).ToList();
            
            // You can pass List<Component> directly, or use a ViewModel if you need more data for the home page
            return View(featuredComponents);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            // Logic for your contact page
            return View();
        }

        public IActionResult Feedback()
        {
            // Logic for your feedback page
            return View();
        }
        
        [HttpPost]
        public IActionResult SubmitFeedback(FeedbackViewModel model) // Assuming you create a FeedbackViewModel
        {
            if(ModelState.IsValid)
            {
                // Process feedback (e.g., save to a file, database, or send an email)
                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("Index");
            }
            // If model state is invalid, return to the feedback form with errors
            return View("Feedback", model);
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  

        // Helper method to load all components from all JSON files
        // This is similar to the logic you might have in ComputersController or a shared service
         private List<Component> LoadAllSystemComponents()
        {
            var allComponents = new List<Component>();
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            var componentFileName = "component.json"; 
            var filePath = Path.Combine(baseDir, componentFileName);

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                try
                {
                    var components = JsonConvert.DeserializeObject<List<Component>>(json);
                    if (components != null)
                    {
                        // Type should be directly from the JSON. If c.Type is null/empty here,
                        // it means the "type" field is missing for that component in component.json.
                        allComponents.AddRange(components);
                    }
                }
                catch (JsonSerializationException ex)
                {
                    // Log this error or handle it appropriately
                    Debug.WriteLine($"Error deserializing {componentFileName}: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine($"Error: {componentFileName} not found in {baseDir}");
            }
            return allComponents;
        }
    }
}