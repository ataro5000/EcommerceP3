namespace ComputerBuilderMvcApp.Models
{
    public class Review
    {
        public int ID { get; set; }
        public string? ItemId { get; set; }
        public decimal Rating { get; set; } 
        public string? Comments { get; set; } 
        public string? CustomerName { get; set; } 
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    }
}