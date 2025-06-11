// This file defines the BuilderController class, which is responsible for handling computer building functionalities.
// It allows users to select components for a custom computer build, calculates the total price, and adds the build to the shopping cart.
using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;


namespace ComputerBuilderMvcApp.Controllers
{

    /// Controller responsible for the computer building process.

    public class BuilderController(Cart cart) : Controller
    {
        private readonly Cart _cart = cart;


        /// Displays the computer builder page.
        /// It loads available components for predefined categories and initializes the view model.

        /// A list of component categories to display. If null or empty, default categories are used.
        /// The view for the computer builder page, populated with component data.
        public IActionResult Index(List<string> categories)
        {
            var allComponents = ComponentsController.LoadComponents(categories);
            var viewModel = new ComputerBuilder
            {
                ComponentCategories = ["CPU", "Motherboard", "RAM", "GPU", "Storage", "PSU", "Case"]
            };
      
            foreach (var category in viewModel.ComponentCategories)
            {
                if (!viewModel.AvailableComponentsByType.ContainsKey(category))
                {
                    viewModel.AvailableComponentsByType[category] = [];
                }
                if (allComponents != null)
                {
                    viewModel.AvailableComponentsByType[category] = [.. allComponents.Where(c => c.Type != null && c.Type.Equals(category, StringComparison.OrdinalIgnoreCase))];
                }
              

                if (!viewModel.SelectedComponentIds.ContainsKey(category))
                {
                    viewModel.SelectedComponentIds[category] = null;
                   
                }
            }
            viewModel.TotalPrice = CalculateBuildPrice(viewModel.SelectedComponentIds, allComponents ?? []);

            return View(viewModel);
        }


        /// Handles the submission of a computer build.
        /// It validates the submitted build, adds the selected components to the cart, and redirects the user.

        /// The ComputerBuilder model containing the user's selected components.

        /// Redirects to the cart index page if the build is successfully added.
        /// Redirects back to the builder index page with an error message if no components are selected or if no valid components are found.

        [HttpPost]
        public IActionResult BuildAndAddToCart(ComputerBuilder submittedBuild)
        {

            if (submittedBuild.ComponentCategories == null || submittedBuild.ComponentCategories.Count == 0)
            {
                submittedBuild.ComponentCategories = ["CPU", "Motherboard", "RAM", "GPU", "Storage", "PSU", "Case"];
            }

            submittedBuild.AvailableComponentsByType ??= [];

            if (submittedBuild.SelectedComponentIds == null || !submittedBuild.SelectedComponentIds.Values.Any(id => !string.IsNullOrEmpty(id)))
            {
                TempData["ErrorMessage"] = "Please select at least one component for your build.";
            }

            var allSystemComponents = ComponentsController.LoadComponents(submittedBuild.ComponentCategories);

            foreach (var selection in submittedBuild.SelectedComponentIds ?? [])
            {
                if (!string.IsNullOrEmpty(selection.Value))
                {
                    var component = allSystemComponents.FirstOrDefault(c => c.Id == selection.Value &&
                                                                         c.Type != null &&
                                                                         c.Type.Equals(selection.Key, StringComparison.OrdinalIgnoreCase));
                    if (component != null)
                    {

                        _cart.AddItem(component, 1);
                        SessionCart.SaveCart(HttpContext.Session, _cart);

                    }
                }
            }
            if (_cart.Items.Count == 0)
            {
                TempData["ErrorMessage"] = "No valid components were selected or found for your build.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Cart", new { message = "Build added to cart successfully!" });
        }


        /// Calculates the total price of the selected components in a build.

        /// A dictionary mapping component categories to the IDs of selected components.
        /// A list of all available components.
        /// The total price of the selected components in currency (e.g., dollars).
        private static decimal CalculateBuildPrice(Dictionary<string, string?> selectedIds, List<Component> allComponents)
        {
            decimal totalPriceInCurrency = 0; 
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
                        totalPriceInCurrency += component.PriceCents / 100.0m; 
                    }
                }
            }
            return totalPriceInCurrency;
        }
      
    }
}