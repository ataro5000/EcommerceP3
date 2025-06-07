using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerBuilderMvcApp.Models
{
    public class CartItem
    {
        public int ComputerId { get; set; }
        public string ComputerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price per unit at the time of adding to cart
        public decimal Subtotal => Quantity * Price;
    }
}