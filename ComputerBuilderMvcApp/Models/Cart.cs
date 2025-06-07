using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerBuilderMvcApp.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalAmount => Items.Sum(item => item.Subtotal);

        // For adding individual components (this method remains largely the same)
        public void AddItem(Component component, int quantity = 1)
        {
            if (component == null || string.IsNullOrEmpty(component.Id)) return;

            // Now, every item is treated as an individual component, so IsCustomBuild check is removed.
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
                    CartItemPriceCents = component.PriceCents, // Store cents for one unit
                    CartItemQuantity = quantity
                    // IsCustomBuild = false; // No longer needed
                });
            }
        }

        // This method is now significantly different.
        // It iterates through the components of a "build" and adds them individually.
        public void AddBuiltComputerToCart(List<Component> componentsInBuild, string buildNameForReferenceOnly = "")
        {
            if (componentsInBuild == null || !componentsInBuild.Any()) return;

            // Optional: You could log the buildNameForReferenceOnly or associate it with the order later,
            // but it won't be part of the CartItem structure itself.

            foreach (var component in componentsInBuild)
            {
                // Add each component from the build as a separate item in the cart.
                // We assume quantity 1 for each component part of a single build.
                // If a component from the build is already in the cart (e.g., user added it separately),
                // this will increment its quantity.
                AddItem(component, 1);
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