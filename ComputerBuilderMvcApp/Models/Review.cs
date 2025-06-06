namespace ComputerBuilderMvcApp.Models
{
    public class Review
    {
        public int ID { get; set; }
        public int ComputerID { get; set; }
        public int Rating { get; set; } // Rating out of 5
        public string Comments { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}