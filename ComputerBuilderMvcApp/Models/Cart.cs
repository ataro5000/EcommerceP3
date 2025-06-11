namespace ComputerBuilderMvcApp.Models
{
    public class Cart
    {
        public Cart()
        {

        }   
        public List<CartItem> Items { get; set; } = [];

        // TotalAmount should be in the main currency unit (e.g., dollars)
        public decimal TotalAmountAsCurrency => Items.Sum(item => item.SubtotalAsCurrency);
        
        public decimal TotalAmountBeforeTaxe => Items.Sum(item => item.SubtotalInCents / 100.0m);
        
        public void AddItem(Component component, int quantity = 1)
        {
            if (component == null || string.IsNullOrEmpty(component.Id)) return;

            var existingItem = Items.FirstOrDefault(i => i.CartItemId == component.Id);
            if (existingItem != null)
            {
                existingItem.CartItemQuantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    CartItemId = component.Id,
                    CartItemImage = component.Image,
                    CartItemName = component.Name,
                    CartItemPriceCents = component.PriceCents,
                    CartItemQuantity = quantity
                });
            }
        }

        public void AddBuiltComputerToCart(List<Component> componentsInBuild)
        {
            if (componentsInBuild == null || componentsInBuild.Count == 0) return;

            foreach (var component in componentsInBuild)
            {
                AddItem(component, 1) ; 
            }
        }

        public void RemoveItem(string cartItemId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}