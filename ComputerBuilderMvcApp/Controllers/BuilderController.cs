using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ComputerBuilderMvcApp.ViewModels;
using Microsoft.AspNetCore.Http; // For ISession
using System; // For StringComparison

// ... (existing using statements) ...

namespace ComputerBuilderMvcApp.Controllers
{
    public class BuilderController(Cart cart) : Controller // Assuming standard constructor injection for Cart
    {
        private readonly Cart _cart = cart;

        public IActionResult Index()
        {
            var viewModel = new ComputerViewModel
            {
                ComponentCategories = ["CPU", "Motherboard", "RAM", "GPU", "Storage", "PSU", "Case"]
            };
            var allComponents = LoadAllSystemComponents();

            foreach (var category in viewModel.ComponentCategories)
            {
                if (!viewModel.AvailableComponentsByType.ContainsKey(category))
                {
                    viewModel.AvailableComponentsByType[category] = [];
                }
                viewModel.AvailableComponentsByType[category] = [.. allComponents.Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))];
            }
            // viewModel.TotalPrice will be in decimal currency (e.g., dollars)
            viewModel.TotalPrice = CalculateBuildPrice(viewModel.SelectedComponentIds, allComponents);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult BuildAndAddToCart(ComputerViewModel submittedBuild)
        {
            // Ensure ComponentCategories is initialized if not submitted or empty
            if (submittedBuild.ComponentCategories == null || submittedBuild.ComponentCategories.Count == 0)
            {
                submittedBuild.ComponentCategories = new List<string> { "CPU", "Motherboard", "RAM", "GPU", "Storage", "PSU", "Case" };
            }
            // Ensure AvailableComponentsByType is initialized for the view model if returning due to error
            if (submittedBuild.AvailableComponentsByType == null) // Check for null, not just Any() if it might not be initialized
            {
                submittedBuild.AvailableComponentsByType = new Dictionary<string, List<Component>>();
            }

            if (submittedBuild.SelectedComponentIds == null || !submittedBuild.SelectedComponentIds.Values.Any(id => !string.IsNullOrEmpty(id)))
            {
                TempData["ErrorMessage"] = "Please select at least one component for your build.";
                var allComponentsForViewOnError = LoadAllSystemComponents();
                foreach (var category in submittedBuild.ComponentCategories)
                {
                    if (!submittedBuild.AvailableComponentsByType.ContainsKey(category))
                    {
                        submittedBuild.AvailableComponentsByType[category] = new List<Component>();
                    }
                    submittedBuild.AvailableComponentsByType[category] = [.. allComponentsForViewOnError.Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))];
                }
                submittedBuild.TotalPrice = CalculateBuildPrice(submittedBuild.SelectedComponentIds ?? [], allComponentsForViewOnError);
                return View("Index", submittedBuild);
            }

            var allSystemComponents = LoadAllSystemComponents();
            List<Component> chosenComponents = [];
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

            if (chosenComponents.Count == 0)
            {
                TempData["ErrorMessage"] = "No valid components were selected or found for your build.";
                var allComponentsForViewOnError = LoadAllSystemComponents(); // Renamed for clarity
                foreach (var category in submittedBuild.ComponentCategories)
                {
                    if (!submittedBuild.AvailableComponentsByType.ContainsKey(category)) // Ensure key exists
                    {
                        submittedBuild.AvailableComponentsByType[category] = [];
                    }
                    submittedBuild.AvailableComponentsByType[category] = [.. allComponentsForViewOnError.Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))];
                }
                submittedBuild.TotalPrice = CalculateBuildPrice(submittedBuild.SelectedComponentIds, allComponentsForViewOnError);
                return View("Index", submittedBuild);
            }

            _cart.AddBuiltComputerToCart(chosenComponents, submittedBuild.Name ?? "Custom PC Build Components");
            SessionCart.SaveCart(HttpContext.Session, _cart);
            TempData["SuccessMessage"] = $"Components for '{submittedBuild.Name ?? "Custom PC Build"}' have been added to your cart.";
            return RedirectToAction("Index", "Cart");
        }

            
            // Save cart to session after modification

        private static decimal CalculateBuildPrice(Dictionary<string, string?> selectedIds, List<Component> allComponents)
        {
            decimal totalPriceInCurrency = 0; // This will be in dollars/euros
            if (selectedIds == null) return 0;

            foreach (var selection in selectedIds)
            {
                if (!string.IsNullOrEmpty(selection.Value))
                {
                    var component = allComponents.FirstOrDefault(c => c.Id == selection.Value &&
                                                                     c.Type != null &&
                                                                     c.Type.Equals(selection.Key, StringComparison.OrdinalIgnoreCase));
                    if (component != null)
                    {
                        totalPriceInCurrency += (component.PriceCents / 100.0m); // Convert cents to currency here
                    }
                }
            }
            return totalPriceInCurrency;
        }

        private static List<Component> LoadAllSystemComponents()
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
                        // DO NOT MODIFY PriceCents here. It should remain in cents.
                        // If you had a 'Price' property, you would calculate it:
                        // components.ForEach(c => c.Price = c.PriceCents / 100.0m);
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