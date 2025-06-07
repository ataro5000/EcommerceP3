using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerBuilderMvcApp.Models
{
    public class CartItem
    {
        public string? CartItemId { get; set; } // Component.Id
        public string? CartItemName { get; set; }
        public int CartItemQuantity { get; set; }
        public decimal CartItemPriceCents { get; set; } // Price per unit IN CENTS

        // Subtotal for this cart item (Quantity * Unit Price in CENTS)
        // To display as currency, divide by 100 in the view
        public decimal SubtotalInCents => CartItemQuantity * CartItemPriceCents;

        // Helper for display in views, if needed, or do conversion in view
        public decimal SubtotalAsCurrency => SubtotalInCents / 100.0m;
    }
}