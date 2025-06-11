using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComponentsController : Controller
    {

        public IActionResult Index(List<string> categories)
        {
            
            var allReviews = LoadAllReviews();
             var components = LoadComponents(categories);
            foreach (var component in components)
            {
                if (component.Id != null) component.Reviews = [.. allReviews.Where(r => r.ItemId == component.Id)];
                
            }

            ViewData["SelectedCategories"] = categories ?? [];
            return View(components);
        }


        public IActionResult Details(List<string> categories, string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Component ID cannot be null or empty.");
            
            var allReviews = LoadAllReviews(); 
            var allLoadedComponents = LoadComponents(categories); 
            var component = allLoadedComponents.FirstOrDefault(c => c.Id == id);

            if (component == null) return NotFound($"Component with ID '{id}' not found.");
            
           
            if (component.Id != null) component.Reviews = [.. allReviews.Where(r => r.ItemId == component.Id)];
            
            return View(component);
        }

     

        public static List<Component> LoadComponents(List<string> categoriesToFilter)
        {
            var allLoadedComponents = new List<Component>();
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            var componentFileName = "component.json"; // Single source file
            var filePath = Path.Combine(baseDir, componentFileName);

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                try
                {
                    var componentsFromFile = JsonConvert.DeserializeObject<List<Component>>(json);
                    if (componentsFromFile != null) allLoadedComponents.AddRange(componentsFromFile);
                }
                catch (JsonSerializationException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deserializing {componentFileName}: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Error: {componentFileName} not found in {baseDir}");
            }
            if (categoriesToFilter != null && categoriesToFilter.Count != 0)
            {
                var lowerCategoriesToFilter = categoriesToFilter.Select(c => c.ToLowerInvariant()).ToList();
                return [.. allLoadedComponents.Where(c => c.Type != null && lowerCategoriesToFilter.Contains(c.Type.ToLowerInvariant()))];
            }
            return allLoadedComponents;
        }

        private static List<Review> LoadAllReviews()
        {
            string dataDirPath = Path.Combine(Directory.GetCurrentDirectory(), "Data"); // Adjust if needed
            string reviewsFilePath = Path.Combine(dataDirPath, "reviews.json");

            if (!System.IO.File.Exists(reviewsFilePath)) return [];
            
            var json = System.IO.File.ReadAllText(reviewsFilePath);
            if (string.IsNullOrWhiteSpace(json)) return [];
            
            return System.Text.Json.JsonSerializer.Deserialize<List<Review>>(json) ?? [];
        }
    }
}