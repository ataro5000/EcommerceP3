namespace ComputerBuilderMvcApp.Models
{
    public class Component
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; } 
        public decimal PriceCents { get; set; } 
        public string? Spec { get; set; }
        public string? Image { get; set; } 
    }
}