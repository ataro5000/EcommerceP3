using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerBuilderMvcApp.Models
{
    public class CartItem
    {
        public string? CartItemId { get; set; }
        public string? CartItemName { get; set; }
        public int CartItemQuantity { get; set; }
        public decimal CartItemPriceCents { get; set; } // Price per unit at the time of adding to cart
        public decimal Subtotal => CartItemPriceCents / 100;
    }
}