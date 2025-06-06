using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using ComputerBuilderMvcApp.ViewModels;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComputersController : Controller
    {
        private readonly string _computersDataFilePath; // Declare the field

        public ComputersController() // Constructor
        {
            // Initialize the field
            _computersDataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "computers.json");
        }

        public IActionResult Index()
        {
            List<ComputerViewModel> computerViewModels = new List<ComputerViewModel>();
            if (System.IO.File.Exists(_computersDataFilePath)) // Use the field
            {
                var json = System.IO.File.ReadAllText(_computersDataFilePath); // Use the field
                var computers = JsonConvert.DeserializeObject<List<Computer>>(json);
                if (computers != null)
                {
                    foreach (var comp in computers)
                    {
                        // This is a simplified mapping. You might want a more robust one.
                        computerViewModels.Add(new ComputerViewModel
                        {
                            Id = comp.ID,
                            Name = comp.Name ?? "N/A",
                            TotalPrice = comp.TotalPrice,
                            StandardComponents = comp.StandardComponents ?? new List<Component>()
                        });
                    }
                }
            }
            return View(computerViewModels);
        }

        private Computer? GetComputerById(int id)
        {
            if (!System.IO.File.Exists(_computersDataFilePath)) // Use the field
            {
                return null;
            }
            var json = System.IO.File.ReadAllText(_computersDataFilePath); // Use the field
            var computers = JsonConvert.DeserializeObject<List<Computer>>(json);
            // The warning CS8603 was likely here. Adding a null check for 'computers'
            // and ensuring FirstOrDefault is understood to potentially return null.
            return computers?.FirstOrDefault(c => c.ID == id);
        }

        public IActionResult Details(int id)
        {
            var computer = GetComputerById(id);
            if (computer == null)
            {
                return NotFound();
            }

            // Example of mapping to a ViewModel for the Details view
            var viewModel = new ComputerViewModel
            {
                Id = computer.ID,
                Name = computer.Name ?? "N/A",
                TotalPrice = computer.TotalPrice,
                StandardComponents = computer.StandardComponents ?? new List<Component>(),
                // You would also load available components for customization here
                // AvailableComponents = LoadAvailableComponentsFor(computer.ID) // Example
            };
            return View(viewModel);
        }
    }
}