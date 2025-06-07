using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerBuilderMvcApp.Models
{
    public class CartItem
    {
        public string? CartId { get; set; }
        public string? CartName { get; set; }
        public int CartQuantity { get; set; }
        public decimal CartPriceCents { get; set; } // Price per unit at the time of adding to cart
        public decimal Subtotal => CartQuantity * (CartPriceCents / 100);
    }
}