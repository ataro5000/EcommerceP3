// This file defines the CartItem class, which represents an individual item within a shopping cart.
// It includes properties for the item's ID, name, image, quantity, price, and calculated subtotal.
namespace ComputerBuilderMvcApp.Models
{
    public class CartItem
    {
        // Constructor for the CartItem class.
        public CartItem () {}
        // Gets or sets the unique identifier for the cart item.
        public string? CartItemId { get; set; } 
        // Gets or sets the name of the cart item.
        public string? CartItemName { get; set; }
        // Gets or sets the image URL or path for the cart item.
        public string? CartItemImage { get; set; }
        // Gets or sets the quantity of this item in the cart.
        public int CartItemQuantity { get; set; }
        // Gets or sets the price of a single unit of this item in cents.
        public decimal CartItemPriceCents { get; set; } 
        // Calculates the subtotal for this cart item in cents (quantity * price).
        public decimal SubtotalInCents => CartItemQuantity * CartItemPriceCents;
        // Calculates the subtotal for this cart item in the main currency unit (e.g., dollars), including a 15% tax.
        public decimal SubtotalAsCurrency => SubtotalInCents * 1.15m / 100.0m;
    }
}