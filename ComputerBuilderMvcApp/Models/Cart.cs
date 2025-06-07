using System.Collections.Generic;
using System.Linq;

namespace ComputerBuilderMvcApp.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = [];

        public decimal TotalAmount => Items.Sum(item => item.Subtotal);

        public void AddItem(Component component, int quantity = 1)
        {
            if (component == null) return;

            var existingItem = Items.FirstOrDefault(i =>
                i.CartId == component.Id);
            if (existingItem != null)
            {
                existingItem.CartQuantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    CartId = component.Id,
                    CartName = component.Name, 
                    CartPriceCents = component.PriceCents, 
                    CartQuantity = quantity
                });
            }
        }

        public void RemoveItem(int CartId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.CartId == CartId.ToString());
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