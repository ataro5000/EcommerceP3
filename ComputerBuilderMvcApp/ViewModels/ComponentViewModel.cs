using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace ComputerBuilderMvcApp.ViewModels
{
    public class ComponentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Custom Build";
        public Dictionary<string, List<Component>> AvailableComponentsByType { get; set; } = [];

        // To store the IDs of the components currently selected by the user for this build
        // Key: Component Type (e.g., "CPU", "RAM"), Value: Selected Component ID (nullable if nothing selected for a category)
        public Dictionary<string, string> SelectedComponentIds { get; set; } = [];

        // Dynamically calculated total price of selected components
        public decimal TotalPriceCents { get; set; }

        // List of component categories to build the form dynamically (e.g., "CPU", "GPU", "RAM")
        // This should match the keys in AvailableComponentsByType and SelectedComponentIds
        public List<string> ComponentCategories { get; set; } =
        [
            "CPU", "Motherboard", "RAM", "GPU", "Storage", "PSU", "Case" 
            // Add or remove categories as per your JSON files
        ];
    }
}