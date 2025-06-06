namespace ComputerBuilderMvcApp.Models
{
    public class Component
    {
        public int Id { get; set; } 
        public string Type { get; set; } = string.Empty;// "RAM", "HDD", "CPU", "GPU", "PSU", "Motherboard"
        public string Spec { get; set; } = string.Empty;// e.g., "16GB DDR4", "Intel i7"
        public decimal Price { get; set; } // rounded to 2 decimal places
    }
}