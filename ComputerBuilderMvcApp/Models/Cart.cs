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
                    CartItemName = component.Name,
                    CartItemPriceCents = component.PriceCents, // Store PriceCents directly
                    CartItemQuantity = quantity
                });
            }
        }

        public void AddBuiltComputerToCart(List<Component> componentsInBuild, string buildNameForReferenceOnly = "")
        {
            if (componentsInBuild == null || !componentsInBuild.Any()) return;

            foreach (var component in componentsInBuild)
            {
                AddItem(component, 1); // Assumes quantity 1 for each part of a build
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