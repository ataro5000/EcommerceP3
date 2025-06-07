using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Linq; // Required for FirstOrDefault
using System.Collections.Generic; // Required for List
using System.IO; // Required for Path
using Newtonsoft.Json; // Required for JsonConvert
using Microsoft.AspNetCore.Http; // For ISession
using System; // Required for Guid
using System.Diagnostics; // Required for Debug

namespace ComputerBuilderMvcApp.Controllers
{
    public class CartController(Cart cart) : Controller
    {
        private readonly Cart _cart = cart;

        public IActionResult Index()
        {
            return View(_cart);
        }

       [HttpPost]
        public IActionResult AddSingleComponentToCart(string componentId, int quantity = 1)
        {
            Debug.WriteLine($"[CartController.AddSingleComponentToCart] ACTION ENTERED. ComponentId: {componentId}, Quantity: {quantity}"); // <-- ADD THIS

            if (string.IsNullOrEmpty(componentId))
            {
                Debug.WriteLine("[CartController.AddSingleComponentToCart] ComponentId is NULL or EMPTY."); // <-- ADD THIS
                TempData["ErrorMessage"] = "Component ID is missing.";
                string refererUrl = Request.Headers.Referer.FirstOrDefault() ?? string.Empty; 
                return Redirect(refererUrl ?? Url.Action("Index", "Components") ?? "/"); 
            }
            if (quantity < 1) quantity = 1;

            Debug.WriteLine("[CartController.AddSingleComponentToCart] Attempting to get component..."); // <-- ADD THIS
            var component = GetSystemComponentById(componentId); // Make sure this method exists and works

            if (component != null)
            {
                Debug.WriteLine($"[CartController.AddSingleComponentToCart] Component FOUND: {component.Name}. Attempting to add to cart object."); // <-- ADD THIS
                _cart.AddItem(component, quantity); // This is where the cart object is modified
                Debug.WriteLine($"[CartController.AddSingleComponentToCart] Item added to _cart object. _cart.Items.Count now: {_cart.Items.Count}"); // <-- ADD THIS
                
                Debug.WriteLine("[CartController.AddSingleComponentToCart] Attempting to SAVE CART to session..."); // <-- ADD THIS
                SessionCart.SaveCart(HttpContext.Session, _cart); // <<<< THIS IS THE CRITICAL CALL
                Debug.WriteLine("[CartController.AddSingleComponentToCart] SessionCart.SaveCart CALLED."); // <-- ADD THIS

                TempData["SuccessMessage"] = $"{component.Name} (x{quantity}) added to cart.";
            }
            else
            {
                Debug.WriteLine("[CartController.AddSingleComponentToCart] Component NOT FOUND."); // <-- ADD THIS
                TempData["ErrorMessage"] = "Component not found.";
            }
            string currentRefererUrl = Request.Headers.Referer.FirstOrDefault() ?? string.Empty; 
            Debug.WriteLine($"[CartController.AddSingleComponentToCart] Redirecting to: {currentRefererUrl}"); // <-- ADD THIS
            return Redirect(currentRefererUrl ?? Url.Action("Index", "Cart") ?? "/"); 
        }

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            int itemCount = _cart.Items.Sum(item => item.CartItemQuantity);
            System.Diagnostics.Debug.WriteLine($"[CartController] GetCartItemCount called. Items in cart object: {_cart.Items.Count}, Total quantity: {itemCount}"); // DEBUG LINE
            return Json(new { itemCount = itemCount });
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
            SessionCart.SaveCart(HttpContext.Session, _cart);

            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            if (!_cart.Items.Any())
            {
                SessionCart.SaveCart(HttpContext.Session, _cart);
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
            SessionCart.SaveCart(HttpContext.Session, _cart); // <-- ADD THIS LINE
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