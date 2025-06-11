// This file defines the CartController class, which manages the shopping cart functionality.
// It handles adding, removing, and viewing items in the cart, as well as processing orders and displaying order confirmations.
using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json; 


namespace ComputerBuilderMvcApp.Controllers
{
    public class CartController(Cart cart) : Controller
    {
        private readonly Cart _cart = cart;

        // Displays the shopping cart page.
        public IActionResult Index()
        {
            return View(_cart);
        }

        // Adds a single component to the shopping cart.
        // Expects a componentId and an optional quantity.
        // Returns a JSON response indicating success or failure.
        [HttpPost]
        public JsonResult AddSingleComponentToCart(string componentId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(componentId))
            {
                return Json(new { success = false, message = "Component ID is missing." });
            }

            var component = GetSystemComponentById(componentId);

            if (component != null)
            {
                _cart.AddItem(component, quantity);
                SessionCart.SaveCart(HttpContext.Session, _cart);
                return Json(new { success = true, message = $"{component.Name} (x{quantity}) added to cart." });
            }
            else
            {
                return Json(new { success = false, message = "Component not found." });
            }
        }

        // Retrieves the current number of items in the cart and the total price.
        // Returns a JSON response with the item count and total cart price.
        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            int itemCount = _cart.Items.Sum(item => item.CartItemQuantity);
            string totalCartPrice = _cart.TotalAmountBeforeTaxe.ToString("C");
            return Json(new { itemCount, totalCartPrice });
        }

        // Removes an item from the shopping cart based on its cartItemId.
        // Redirects to the cart index page with a success or error message.
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

        // Displays the checkout page.
        // If the cart is empty, it redirects to the cart index page with an error message.
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

        // Processes the order.
        // If the cart is empty, it redirects to the cart index page with an error message.
        // Otherwise, it clears the cart, generates an order ID, and redirects to the order confirmation page.
        [HttpPost]
        public IActionResult ProcessOrder()
        {
            if (!_cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Cannot process order for an empty cart.";
                return RedirectToAction("Index");
            }
            var orderId = Guid.NewGuid().ToString()[..8].ToUpper();
            _cart.Clear();
            SessionCart.SaveCart(HttpContext.Session, _cart);
            TempData["SuccessMessage"] = $"Order {orderId} placed successfully!";
            return RedirectToAction("OrderConfirmation", new { id = orderId });
        }

        // Displays the order confirmation page.
        // Takes an order ID as a parameter.
        public IActionResult OrderConfirmation(string id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        // Retrieves a system component by its ID from the component.json data file.
        // Returns the Component object if found, otherwise null.
        private static Component? GetSystemComponentById(string componentId)
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