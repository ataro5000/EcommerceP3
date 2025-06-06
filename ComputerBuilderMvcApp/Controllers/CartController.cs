using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


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

        public IActionResult AddToCart(int computerId)
        {
            // Logic to add the computer to the cart
            // This would typically involve fetching the computer details from a data source
            // and adding it to the cart.

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int computerId)
        {
            // Logic to remove the computer from the cart

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            // Logic to handle the checkout process

            return View();
        }

        public IActionResult OrderConfirmation()
        {
            // Logic to display order confirmation

            return View();
        }
    }
}