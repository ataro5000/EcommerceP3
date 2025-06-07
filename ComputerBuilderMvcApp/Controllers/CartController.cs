using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Linq; // Required for FirstOrDefault
using System.Collections.Generic; // Required for List
using System.IO; // Required for Path
using Newtonsoft.Json; // Required for JsonConvert
using System; // Required for Guid

namespace ComputerBuilderMvcApp.Controllers
{
    public class CartController : Controller
    {
        private readonly Cart _cart;

        public CartController(Cart cart)
        {
            _cart = cart;
        }

        public IActionResult Index()
        {
            return View(_cart);
        }

        [HttpPost]
        public IActionResult AddSingleComponentToCart(string componentId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(componentId))
            {
                TempData["ErrorMessage"] = "Component ID is missing.";
                string refererUrl = Request.Headers.Referer.FirstOrDefault() ?? string.Empty; // Handle possible null value
                return Redirect(refererUrl ?? Url.Action("Index", "Components") ?? "/"); // Fallback chain
            }
            if (quantity < 1) quantity = 1;

            var component = GetSystemComponentById(componentId);

            if (component != null)
            {
                _cart.AddItem(component, quantity);
                TempData["SuccessMessage"] = $"{component.Name} (x{quantity}) added to cart.";
            }
            else
            {
                TempData["ErrorMessage"] = "Component not found.";
            }
            string currentRefererUrl = Request.Headers.Referer.FirstOrDefault() ?? string.Empty; // Use FirstOrDefault
            return Redirect(currentRefererUrl ?? Url.Action("Index", "Cart") ?? "/"); // Fallback chain
        }

        [HttpPost]
        public IActionResult RemoveFromCart(string cartItemId)
        {
            if (string.IsNullOrEmpty(cartItemId))
            {
                 TempData["ErrorMessage"] = "Cart item ID is missing.";
                 return RedirectToAction("Index");
            }
            _cart.RemoveItem(cartItemId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            if (!_cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }
            // Potentially pass cart to checkout view if it needs to display items again
            return View(_cart);
        }

        [HttpPost]
        public IActionResult ProcessOrder() // Simplified
        {
            if (!_cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Cannot process order for an empty cart.";
                return RedirectToAction("Index");
            }

            // Here you would typically save the order to a database, process payment, etc.
            // For this example, we'll just generate an order ID and clear the cart.
            var orderId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            _cart.Clear();
            TempData["SuccessMessage"] = $"Order {orderId} placed successfully!";
            return RedirectToAction("OrderConfirmation", new { id = orderId });
        }

        public IActionResult OrderConfirmation(string id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        // Helper to load a single component by ID.
        // In a larger app, this would be part of a data service.
        private Component? GetSystemComponentById(string componentId)
        {
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
                        // PriceCents should already be in cents from the JSON.
                        // No division needed here.
                        return components.FirstOrDefault(c => c.Id == componentId);
                    }
                }
                catch (JsonSerializationException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deserializing {componentFileName} in CartController: {ex.Message}");
                }
            }
            return null;
        }
    }
}