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
        public decimal SubtotalInCents => CartItemQuantity * CartItemPriceCents;
        public decimal SubtotalAsCurrency => SubtotalInCents * 1.15m / 100.0m;
    }
}