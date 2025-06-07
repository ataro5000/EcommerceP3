// filepath: c:\School\COMP466\TMA3A\EcommerceP3\ComputerBuilderMvcApp\Controllers\ComponentsController.cs
using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComponentsController : Controller
    {
        public IActionResult Index(string category = "all") // Allow filtering by category
        {
            var components = LoadComponents(category);
            // You might want a ViewModel if you need to pass more than just the list
            return View(components);
        }

        private List<Component> LoadComponents(string category)
        {
            var allComponents = new List<Component>();
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            var fileNames = new List<string>();

            if (string.IsNullOrEmpty(category) || category.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                fileNames.AddRange(new[] { "cpus.json", "gpus.json", "motherboards.json", "psus.json", "ram_modules.json" });
            }
            else
            {
                // Ensure the category maps to a valid file name to prevent directory traversal issues
                var safeCategory = category.ToLowerInvariant();
                if (new[] { "cpus", "gpus", "motherboards", "psus", "ram_modules" }.Contains(safeCategory))
                {
                     fileNames.Add($"{safeCategory}.json");
                }
            }

            foreach (var fileName in fileNames)
            {
                var filePath = Path.Combine(baseDir, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    var json = System.IO.File.ReadAllText(filePath);
                    var components = JsonConvert.DeserializeObject<List<Component>>(json);
                    if (components != null)
                    {
                        // Optionally set the Type if not in JSON, though it's better if JSON includes it
                        // string typeFromFile = Path.GetFileNameWithoutExtension(fileName).Replace("_modules", "");
                        // components.ForEach(c => c.Type = string.IsNullOrEmpty(c.Type) ? typeFromFile : c.Type);
                        allComponents.AddRange(components);
                    }
                }
            }
            return allComponents;
        }
    }
}