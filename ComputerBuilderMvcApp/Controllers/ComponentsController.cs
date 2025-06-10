using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComponentsController : Controller
    {
        public IActionResult Index(List<string> categories)
        {
            var components = LoadComponents(categories);
            ViewData["SelectedCategories"] = categories ?? [];
            return View(components);
        }

        public IActionResult Details(List<string> categories, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Component ID cannot be null or empty.");
            }

            var allComponents = LoadComponents(categories);
            var component = allComponents.FirstOrDefault(c => c.Id == id);

            if (component == null)
            {
                return NotFound($"Component with ID '{id}' not found.");
            }
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
                    if (componentsFromFile != null)
                    {
                        allLoadedComponents.AddRange(componentsFromFile);
                    }
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
    }
}