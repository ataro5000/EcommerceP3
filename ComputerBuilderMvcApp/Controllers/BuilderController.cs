using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ComputerBuilderMvcApp.ViewModels;
using System; // For StringComparison

namespace ComputerBuilderMvcApp.Controllers
{
    public class BuilderController : Controller
    {
        private readonly Cart _cart;

        public BuilderController(Cart cart)
        {
            _cart = cart;
        }

        // ... Details() action remains the same ...
        // ... CalculateBuildPrice() remains the same ...
        // ... LoadAllSystemComponents() remains the same ...

        [HttpPost]
        public IActionResult BuildAndAddToCart(ComputerViewModel submittedBuild)
        {
            // ... (existing validation and repopulation logic for error cases) ...
            if (submittedBuild.ComponentCategories == null || !submittedBuild.ComponentCategories.Any())
            {
                 submittedBuild.ComponentCategories = new List<string> { "CPU", "Motherboard", "RAM", "GPU", "Storage", "PSU", "Case" };
            }
            if (submittedBuild.AvailableComponentsByType == null || !submittedBuild.AvailableComponentsByType.Any())
            {
                submittedBuild.AvailableComponentsByType = new Dictionary<string, List<Component>>();
            }

            if (submittedBuild.SelectedComponentIds == null || !submittedBuild.SelectedComponentIds.Values.Any(id => !string.IsNullOrEmpty(id)))
            {
                TempData["ErrorMessage"] = "Please select at least one component for your build.";
                // ... (repopulate logic for view) ...
                var allComponentsForViewOnError = LoadAllSystemComponents();
                foreach (var category in submittedBuild.ComponentCategories)
                {
                    if (!submittedBuild.AvailableComponentsByType.ContainsKey(category))
                    {
                        submittedBuild.AvailableComponentsByType[category] = new List<Component>();
                    }
                    submittedBuild.AvailableComponentsByType[category] = allComponentsForViewOnError
                        .Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                submittedBuild.TotalPrice = CalculateBuildPrice(submittedBuild.SelectedComponentIds, allComponentsForViewOnError);
                return View("Details", submittedBuild);
            }

            var allSystemComponents = LoadAllSystemComponents();
            List<Component> chosenComponents = new List<Component>();
            foreach (var selection in submittedBuild.SelectedComponentIds)
            {
                if (!string.IsNullOrEmpty(selection.Value))
                {
                    var component = allSystemComponents.FirstOrDefault(c => c.Id == selection.Value &&
                                                                         c.Type != null &&
                                                                         c.Type.Equals(selection.Key, StringComparison.OrdinalIgnoreCase));
                    if (component != null)
                    {
                        chosenComponents.Add(component);
                    }
                }
            }

            if (!chosenComponents.Any())
            {
                TempData["ErrorMessage"] = "No valid components were selected or found for your build.";
                // ... (repopulate logic for view) ...
                var allComponentsForViewOnError = LoadAllSystemComponents();
                 foreach (var category in submittedBuild.ComponentCategories)
                {
                    if (!submittedBuild.AvailableComponentsByType.ContainsKey(category))
                    {
                        submittedBuild.AvailableComponentsByType[category] = new List<Component>();
                    }
                    submittedBuild.AvailableComponentsByType[category] = allComponentsForViewOnError
                        .Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                submittedBuild.TotalPrice = CalculateBuildPrice(submittedBuild.SelectedComponentIds, allComponentsForViewOnError);
                return View("Details", submittedBuild);
            }

            // MODIFIED PART: Add each chosen component to the cart individually
            _cart.AddBuiltComputerToCart(chosenComponents, submittedBuild.Name ?? "Custom PC Build Components");
            // Alternatively, loop here:
            // foreach (var component in chosenComponents)
            // {
            //     _cart.AddItem(component, 1);
            // }

            TempData["SuccessMessage"] = $"Components for '{submittedBuild.Name ?? "Custom PC Build"}' have been added to your cart.";
            return RedirectToAction("Index", "Cart");
        }
       private decimal CalculateBuildPrice(Dictionary<string, string?> selectedIds, List<Component> allComponents) // Value type changed to string?
        {
            decimal totalPrice = 0;
            if (selectedIds == null) return 0;

            foreach (var selection in selectedIds)
            {
                // selection.Value is now a string?
                if (!string.IsNullOrEmpty(selection.Value))
                {
                    var component = allComponents.FirstOrDefault(c => c.Id == selection.Value && // Direct string comparison
                                                                     c.Type != null &&
                                                                     c.Type.Equals(selection.Key, StringComparison.OrdinalIgnoreCase));
                    if (component != null)
                    {
                        totalPrice += component.PriceCents; // Assuming component.Price is the decimal price
                    }
                }
            }
            return totalPrice;
        }

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
                        // Add a calculated Price property to each component
                        components.ForEach(c => c.PriceCents /= 100.0m);
                        allComponents.AddRange(components);
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
            return allComponents;
        }
    }
}

        
