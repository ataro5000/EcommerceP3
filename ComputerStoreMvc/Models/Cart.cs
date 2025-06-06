public class Cart {
    public List<Component> Components { get; set; }
    public decimal TotalPrice { get; set; }

    public Cart() {
        Components = new List<Component>();
        TotalPrice = 0.0M;
    }

    public void AddComponent(Component component) {
        Components.Add(component);
        TotalPrice += component.Price;
    }

    public void RemoveComponent(Component component) {
        Components.Remove(component);
        TotalPrice -= component.Price;
    }

    public void ClearCart() {
        Components.Clear();
        TotalPrice = 0.0M;
    }
}