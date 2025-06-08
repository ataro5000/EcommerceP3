namespace ComputerBuilderMvcApp.Models
{
    public class ComputerBuilder
    {
        public Dictionary<string, List<Component>> AvailableComponentsByType { get; set; } = [];
        public Dictionary<string, string?> SelectedComponentIds { get; set; } = [];
        public decimal TotalPrice { get; set; }
        public List<string> ComponentCategories { get; set; } = [];
    }
}