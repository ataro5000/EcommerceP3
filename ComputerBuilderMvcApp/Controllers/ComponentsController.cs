// This file defines the ComponentsController class, which is responsible for handling requests related to computer components.
// It loads component data and their reviews from JSON files and provides them to the views.
using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComponentsController : Controller
    {

        // Displays a list of components, optionally filtered by categories.
        // It loads all components and their associated reviews.
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

        // Displays the details of a specific component.
        // It loads the component by its ID and its associated reviews.
        // Returns BadRequest if the ID is null or empty, or NotFound if the component doesn't exist.
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


        // Loads components from the component.json data file.
        // It can filter components based on a list of categories.
        // Returns a list of Component objects.
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

        // Loads all reviews from the reviews.json data file.
        // Returns a list of Review objects. If the file doesn't exist or is empty, an empty list is returned.
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