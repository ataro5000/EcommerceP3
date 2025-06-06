using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ComputerStoreMvc.Models;

namespace ComputerStoreMvc.Controllers
{
    public class ComponentController : Controller
    {
        public IActionResult Index()
        {
            var components = GetComponents();
            return View(components);
        }

        private List<Component> GetComponents()
        {
            var json = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "ram.json"));
            var ramComponents = JsonConvert.DeserializeObject<List<Component>>(json);

            json = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "cpus.json"));
            var cpuComponents = JsonConvert.DeserializeObject<List<Component>>(json);

            json = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "gpus.json"));
            var gpuComponents = JsonConvert.DeserializeObject<List<Component>>(json);

            json = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "motherboards.json"));
            var motherboardComponents = JsonConvert.DeserializeObject<List<Component>>(json);

            json = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "psus.json"));
            var psuComponents = JsonConvert.DeserializeObject<List<Component>>(json);

            var allComponents = new List<Component>();
            allComponents.AddRange(ramComponents);
            allComponents.AddRange(cpuComponents);
            allComponents.AddRange(gpuComponents);
            allComponents.AddRange(motherboardComponents);
            allComponents.AddRange(psuComponents);

            return allComponents;
        }
    }
}