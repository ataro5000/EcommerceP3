using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComponentsController : Controller
    {
        public IActionResult Index()
        {
            var components = LoadComponents("all");
            return View(components);
        }

        private List<Component> LoadComponents(string category)
        {
            var allComponents = new List<Component>();
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");

            if (string.IsNullOrEmpty(category) || category.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                var cpus = JsonConvert.DeserializeObject<List<Component>>(System.IO.File.ReadAllText(Path.Combine(baseDir, "cpus.json")));
                if (cpus != null) allComponents.AddRange(cpus);

                var gpus = JsonConvert.DeserializeObject<List<Component>>(System.IO.File.ReadAllText(Path.Combine(baseDir, "gpus.json")));
                if (gpus != null) allComponents.AddRange(gpus);

                var motherboards = JsonConvert.DeserializeObject<List<Component>>(System.IO.File.ReadAllText(Path.Combine(baseDir, "motherboards.json")));
                if (motherboards != null) allComponents.AddRange(motherboards);

                var psus = JsonConvert.DeserializeObject<List<Component>>(System.IO.File.ReadAllText(Path.Combine(baseDir, "psus.json")));
                if (psus != null) allComponents.AddRange(psus);

                var ram = JsonConvert.DeserializeObject<List<Component>>(System.IO.File.ReadAllText(Path.Combine(baseDir, "ram_modules.json")));
                if (ram != null) allComponents.AddRange(ram);
            }
            else
            {
                var filePath = Path.Combine(baseDir, $"{category.ToLower()}.json");
                if (System.IO.File.Exists(filePath))
                {
                    var components = JsonConvert.DeserializeObject<List<Component>>(System.IO.File.ReadAllText(filePath));
                    if (components != null) allComponents.AddRange(components);
                }
            }
            return allComponents;
        }

    }
}