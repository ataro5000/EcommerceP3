// filepath: c:\School\COMP466\TMA3A\EcommerceP3\ComputerBuilderMvcApp\Controllers\CartController.cs
using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;

using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ComputerBuilderMvcApp.Controllers
{
    public class CartController(Cart cart) : Controller
    {
        private readonly Cart _cart = cart; // Injected Cart service
        private readonly string _computersDataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "component.json");

        public IActionResult Index()
        {
            return View(_cart); // Pass the current cart to the view
        }

        [HttpPost]
        /*public IActionResult AddToCart(string? componentId, int quantity = 1)
        {
            var component = GetComponentById(componentId);
            if (computer != null)
            {
                _cart.AddItem(computer, quantity);
                TempData["SuccessMessage"] = $"{computer.Name} added to cart.";
            }
            else
            {
                TempData["ErrorMessage"] = "Computer not found.";
            }
            // Redirect to the page the user was on, or to the cart, or to computers list
            // For simplicity, redirecting to Computers index. Consider using Request.Headers["Referer"].ToString()
            return RedirectToAction("Index", "Computers"); 
        }*/

        [HttpPost]
        public IActionResult RemoveFromCart(string computerId)
        {
            _cart.RemoveItem(computerId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            if (!_cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checking out.";
                return RedirectToAction("Index");
            }
            // In a real app, you'd pass an OrderViewModel or similar
            return View(); 
        }

        [HttpPost]
        public IActionResult ProcessOrder(/* CheckoutViewModel checkoutModel */)
        {
            if (!_cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Cannot process order for an empty cart.";
                return RedirectToAction("Index");
            }

            // TODO: Implement actual order processing logic:
            // 1. Create an Order object from _cart.Items and checkoutModel details.
            // 2. Save the order (e.g., to a database or another JSON file).
            // 3. Potentially integrate with a payment gateway.
            // 4. Send confirmation emails.

            var orderId = System.Guid.NewGuid().ToString().Substring(0, 8); // Placeholder
            
            _cart.Clear(); // Clear the cart after successful order
            
            TempData["SuccessMessage"] = $"Order {orderId} placed successfully!";
            ViewBag.OrderId = orderId; // Pass to confirmation page
            return RedirectToAction("OrderConfirmation", new { id = orderId });
        }

        public IActionResult OrderConfirmation(string id)
        {
            ViewBag.OrderId = id;
            return View();
        }


    }
}