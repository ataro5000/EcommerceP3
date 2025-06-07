using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;

namespace ComputerBuilderMvcApp.ViewModels
{
    public class ComputerViewModel
    {
        public string Name { get; set; } = "Custom PC Build";

        public Dictionary<string, List<Component>> AvailableComponentsByType { get; set; } = [];

        // Changed Value type from int? to string
        public Dictionary<string, string?> SelectedComponentIds { get; set; } = [];

        public decimal TotalPrice { get; set; }

        public List<string> ComponentCategories { get; set; } = new();
    }
}