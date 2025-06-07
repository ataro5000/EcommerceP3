using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ComputerBuilderMvcApp.ViewModels; // Add this

namespace ComputerBuilderMvcApp.Controllers
{
    public class ComputersController : Controller
    {
        // ... (existing _computersDataFilePath and constructor) ...
        private readonly Cart _cart; // Inject Cart if you need to add to it from here
                                     // Or handle cart addition purely in CartController via redirect

        public ComputersController(Cart cart) // Modified constructor if injecting Cart
        {
            _cart = cart; // If you choose to inject Cart service here
            _computersDataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "computers.json");
        }

        // Action to display the builder interface
        // The 'id' could be for a base computer to start customizing, or ignored for a fresh build
        public IActionResult Details(int? id)
        {
            var viewModel = new ComputerViewModel();

            if (id.HasValue)
            {
                // Optional: Load a base computer configuration if an ID is provided
                var baseComputer = GetComputerById(id.Value);
                if (baseComputer != null)
                {
                    viewModel.Id = baseComputer.ID;
                    viewModel.Name = baseComputer.Name ?? "Custom Build";
                    // Pre-populate SelectedComponentIds from baseComputer.StandardComponents
                    foreach (var comp in baseComputer.StandardComponents)
                    {
                        if (!string.IsNullOrEmpty(comp.Type) && viewModel.ComponentCategories.Contains(comp.Type))
                        {
                            viewModel.SelectedComponentIds[comp.Type] = comp.Id;
                        }
                    }
                }
            }

            // Load all available components and categorize them
            var allComponents = LoadAllSystemComponents(); // You'll need this helper
            foreach (var category in viewModel.ComponentCategories)
            {
                viewModel.AvailableComponentsByType[category] = allComponents
                    .Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Calculate initial total price based on pre-selected components
            viewModel.TotalPrice = CalculateBuildPrice(viewModel.SelectedComponentIds, allComponents);

            return View(viewModel);
        }

        // Action to handle the submission of the selected components and add them to the cart
        [HttpPost]
        public IActionResult BuildAndAddToCart(ComputerViewModel submittedBuild)
        {
            if (submittedBuild.SelectedComponentIds == null || !submittedBuild.SelectedComponentIds.Values.Any(v => v.HasValue && v.Value > 0))
            {
                TempData["ErrorMessage"] = "Please select at least one component for your build.";
                // Repopulate available components before returning to view
                var allComponentsForView = LoadAllSystemComponents();
                foreach (var category in submittedBuild.ComponentCategories)
                {
                    submittedBuild.AvailableComponentsByType[category] = allComponentsForView
                        .Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                submittedBuild.TotalPrice = CalculateBuildPrice(submittedBuild.SelectedComponentIds, allComponentsForView);
                return View("Details", submittedBuild);
            }

            var allSystemComponents = LoadAllSystemComponents();
            List<Component> chosenComponents = new List<Component>();
            foreach (var selection in submittedBuild.SelectedComponentIds)
            {
                if (selection.Value.HasValue && selection.Value.Value > 0)
                {
                    var component = allSystemComponents.FirstOrDefault(c => c.Id == selection.Value.Value && c.Type != null && c.Type.Equals(selection.Key, StringComparison.OrdinalIgnoreCase));
                    if (component != null)
                    {
                        chosenComponents.Add(component);
                    }
                }
            }

            if (!chosenComponents.Any())
            {
                TempData["ErrorMessage"] = "No valid components were selected or found.";
                // Repopulate available components before returning to view (similar to above)
                return View("Details", submittedBuild); // Simplified, ideally repopulate fully
            }

            // Add the list of chosen components to the cart
            // This requires the Cart model to have a method to accept a list of components
            _cart.AddCustomBuild(chosenComponents, submittedBuild.Name ?? "Custom PC Build");

            TempData["SuccessMessage"] = $"'{submittedBuild.Name ?? "Custom PC Build"}' has been added to your cart.";
            return RedirectToAction("Index", "Cart");
        }

        // Helper method to calculate price (can be moved to ViewModel or a service)
        private decimal CalculateBuildPrice(Dictionary<string, int?> selectedIds, List<Component> allComponents)
        {
            decimal totalPrice = 0;
            if (selectedIds == null) return 0;

            foreach (var selection in selectedIds)
            {
                if (selection.Value.HasValue && selection.Value.Value > 0)
                {
                    var component = allComponents.FirstOrDefault(c => c.Id == selection.Value.Value && c.Type != null && c.Type.Equals(selection.Key, StringComparison.OrdinalIgnoreCase));
                    if (component != null)
                    {
                        totalPrice += component.Price;
                    }
                }
            }
            return totalPrice;
        }

        // Helper to load all components from all JSON files (NEEDS TO BE ROBUST)
        // This logic might be better in a dedicated service if used by multiple controllers
        private List<Component> LoadAllSystemComponents()
        {
            var allComponents = new List<Component>();
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            // Define your component JSON files here
            var componentFiles = new[] { "cpus.json", "gpus.json", "motherboards.json", "psus.json", "ram_modules.json", "storage.json", "cases.json" };

            foreach (var fileName in componentFiles)
            {
                var filePath = Path.Combine(baseDir, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    var json = System.IO.File.ReadAllText(filePath);
                    try
                    {
                        var components = JsonConvert.DeserializeObject<List<Component>>(json);
                        if (components != null)
                        {
                            // Infer Type from filename if not present in JSON (less ideal)
                            string inferredType = Path.GetFileNameWithoutExtension(fileName);
                            if (inferredType.EndsWith("s")) inferredType = inferredType.Substring(0, inferredType.Length - 1); // cpus -> cpu
                            if (inferredType == "ram_module") inferredType = "RAM";

                            foreach (var c in components)
                            {
                                if (string.IsNullOrEmpty(c.Type))
                                {
                                    // Basic type inference - make this more robust or ensure JSON has Type
                                    if (fileName.StartsWith("cpus")) c.Type = "CPU";
                                    else if (fileName.StartsWith("gpus")) c.Type = "GPU";
                                    else if (fileName.StartsWith("motherboards")) c.Type = "Motherboard";
                                    else if (fileName.StartsWith("psus")) c.Type = "PSU";
                                    else if (fileName.StartsWith("ram_modules")) c.Type = "RAM";
                                    else if (fileName.StartsWith("storage")) c.Type = "Storage";
                                    else if (fileName.StartsWith("cases")) c.Type = "Case";
                                    // Add more mappings as needed
                                }
                            }
                            allComponents.AddRange(components);
                        }
                    }
                    catch (JsonSerializationException ex)
                    {
                        // Log this error or handle it - indicates a JSON format issue
                        Console.WriteLine($"Error deserializing {fileName}: {ex.Message}");
                    }
                }
            }
            return allComponents;
        }

        // Your existing GetComputerById method
        private Computer? GetComputerById(int id)
        {
            if (!System.IO.File.Exists(_computersDataFilePath)) return null;
            var json = System.IO.File.ReadAllText(_computersDataFilePath);
            var computers = JsonConvert.DeserializeObject<List<Computer>>(json);
            return computers?.FirstOrDefault(c => c.ID == id);
        }
    }
}