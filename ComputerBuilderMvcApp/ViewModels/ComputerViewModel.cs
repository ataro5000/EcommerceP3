using ComputerBuilderMvcApp.Models;

namespace ComputerBuilderMvcApp.ViewModels
{
    public class ComputerViewModel
    {
        public Dictionary<string, List<Component>> AvailableComponentsByType { get; set; } = [];
        public Dictionary<string, string?> SelectedComponentIds { get; set; } = [];
        public decimal TotalPrice { get; set; }
        public List<string> ComponentCategories { get; set; } = new();
    }
}