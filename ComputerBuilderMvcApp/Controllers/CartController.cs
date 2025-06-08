using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json; // Required for JsonConvert


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
            if (string.IsNullOrEmpty(componentId))
            {
                TempData["ErrorMessage"] = "Component ID is missing.";
                string refererUrl = Request.Headers.Referer.FirstOrDefault() ?? string.Empty;
                return Redirect(refererUrl ?? Url.Action("Index", "Components") ?? "/");
            }

            var component = GetSystemComponentById(componentId); 

            if (component != null)
            {
                _cart.AddItem(component, quantity); 
                SessionCart.SaveCart(HttpContext.Session, _cart); 
                TempData["SuccessMessage"] = $"{component.Name} (x{quantity}) added to cart.";
            }
            else
            {
                TempData["ErrorMessage"] = "Component not found.";
            }
            string currentRefererUrl = Request.Headers.Referer.FirstOrDefault() ?? string.Empty;
            SessionCart.SaveCart(HttpContext.Session, _cart); 

            return Redirect(currentRefererUrl ?? Url.Action("Index", "Cart") ?? "/");
        }

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            int itemCount = _cart.Items.Sum(item => item.CartItemQuantity);
            return Json(new { itemCount });
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

        public IActionResult OrderConfirmation(string id)
        {
            ViewBag.OrderId = id;
            return View();
        }

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