// This file defines the ComputerBuilder class, which is a model used in the computer building process.
// It holds the available components categorized by type, the IDs of the selected components,
// the total price of the build, and the list of component categories.
namespace ComputerBuilderMvcApp.Models
{
    public class ComputerBuilder
    {
        // Gets or sets a dictionary where keys are component types (e.g., "CPU", "GPU")
        // and values are lists of available components of that type.
        public Dictionary<string, List<Component>> AvailableComponentsByType { get; set; } = [];

        // Gets or sets a dictionary where keys are component types (e.g., "CPU", "GPU")
        // and values are the IDs of the components selected by the user for that type.
        // The value can be null if no component is selected for a category.
        public Dictionary<string, string?> SelectedComponentIds { get; set; } = [];

        // Gets or sets the total price of the currently configured computer build.
        public decimal TotalPrice { get; set; }

        // Gets or sets the list of component categories to be displayed in the builder (e.g., "CPU", "Motherboard").
        public List<string> ComponentCategories { get; set; } = [];
    }
}