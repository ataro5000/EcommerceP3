// This file defines the Cart class, which represents a shopping cart.
// It manages a list of cart items, calculates the total price, and provides methods for adding, removing, and clearing items.
namespace ComputerBuilderMvcApp.Models
{
    public class Cart
    {
        // Constructor for the Cart class.
        public Cart()
        {
        }   
        // Gets or sets the list of items in the cart.
        public List<CartItem> Items { get; set; } = [];
        // Calculates the total amount of the cart in the main currency unit (e.g., dollars).
        public decimal TotalAmountAsCurrency => Items.Sum(item => item.SubtotalAsCurrency);
        // Calculates the total amount of the cart before taxes, converting from cents to the main currency unit.
        public decimal TotalAmountBeforeTaxe => Items.Sum(item => item.SubtotalInCents / 100.0m);
        // Adds a component to the cart or updates its quantity if it already exists.
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
        // Adds a list of components (representing a built computer) to the cart.
        // Each component is added as a separate item with a quantity of 1.
        public void AddBuiltComputerToCart(List<Component> componentsInBuild)
        {
            if (componentsInBuild == null || componentsInBuild.Count == 0) return;

            foreach (var component in componentsInBuild)
            {
                AddItem(component, 1) ; 
            }
        }
        // Removes an item from the cart based on its cart item ID.
        public void RemoveItem(string cartItemId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }
        // Clears all items from the cart.
        public void Clear()
        {
            Items.Clear();
        }
    }
}