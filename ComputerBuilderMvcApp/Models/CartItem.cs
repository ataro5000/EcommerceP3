namespace ComputerBuilderMvcApp.Models
{
    public class CartItem
    {
        public CartItem () {}
        public string? CartItemId { get; set; } 
        public string? CartItemName { get; set; }
        public string? CartItemImage { get; set; }
        public int CartItemQuantity { get; set; }
        public decimal CartItemPriceCents { get; set; } 
        public decimal SubtotalInCents => CartItemQuantity * (CartItemPriceCents * 1.15m);
        public decimal SubtotalAsCurrency => SubtotalInCents / 100.0m;
    }
}