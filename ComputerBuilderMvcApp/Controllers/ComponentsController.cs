using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComponentsController : Controller
    {
        public IActionResult Index(string category = "all")
        {
            var components = LoadComponents(category);
            return View(components);
        }

        // New Details action for a single component
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Component ID cannot be null or empty.");
            }

            // Load all components to find the one by ID.
            // In a real app with a database, you'd query the DB for a single item.
            var allComponents = LoadComponents("all"); // Get all components
            var component = allComponents.FirstOrDefault(c => c.Id == id);

            if (component == null)
            {
                return NotFound($"Component with ID '{id}' not found.");
            }

            return View(component); // Pass the single Component to the new Details view
        }

        private static List<Component> LoadComponents(string categoryToFilter)
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
                    if (componentsFromFile != null)
                    {
                        componentsFromFile.ForEach(c => {
                            c.PriceCents = c.PriceCents; // Calculate Price from PriceCents
                            // Ensure Type is present in your component.json for each component
                        });
                        allLoadedComponents.AddRange(componentsFromFile);
                    }
                }
                catch (JsonSerializationException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deserializing {componentFileName}: {ex.Message}");
                    // Handle error appropriately
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Error: {componentFileName} not found in {baseDir}");
            }

            // If a specific category was requested (and it's not "all"), filter the results.
            if (!string.IsNullOrEmpty(categoryToFilter) && !categoryToFilter.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return allLoadedComponents.Where(c => c.Type != null && c.Type.Equals(categoryToFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return allLoadedComponents; // Return all if category is "all" or not specified
        }
    }
}