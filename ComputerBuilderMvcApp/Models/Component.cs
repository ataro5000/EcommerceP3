namespace ComputerBuilderMvcApp.Models
{
    public class Component
    {
        public string? Id { get; set; }
        public string? Type { get; set; } // "RAM", "HDD", "CPU", "GPU", "PSU", "Motherboard"
        public string? Name { get; set; } // e.g., "16GB DDR4", "Intel i7"
        public decimal PriceCents { get; set; } // price in cents to avoid floating point issues
        public string? Spec { get; set; }
    }
}