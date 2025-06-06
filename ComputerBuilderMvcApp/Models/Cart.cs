
namespace ComputerBuilderMvcApp.Models
{
    public class Cart
    {
        public List<Component> Items { get; set; } = new List<Component>();
        public decimal TotalPrice { get; set; }

        public void AddItem(Component component)
        {
            Items.Add(component);
            TotalPrice += component.Price;
        }

        public void RemoveItem(Component component)
        {
            Items.Remove(component);
            TotalPrice -= component.Price;
        }

        public void ClearCart()
        {
            Items.Clear();
            TotalPrice = 0;
        }
    }
}