// filepath: c:\School\COMP466\TMA3A\EcommerceP3\ComputerBuilderMvcApp\Models\Cart.cs
using System.Collections.Generic;
using System.Linq;

namespace ComputerBuilderMvcApp.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalAmount => Items.Sum(item => item.Subtotal);

        public void AddItem(Computer computer, int quantity = 1)
        {
            if (computer == null) return;

            var existingItem = Items.FirstOrDefault(i => i.ComputerId == computer.ID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ComputerId = computer.ID,
                    ComputerName = computer.Name ?? "N/A", // Ensure Name is handled if null
                    Price = computer.TotalPrice, // Or a calculated price if customized
                    Quantity = quantity
                });
            }
        }

        public void RemoveItem(int computerId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.ComputerId == computerId);
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