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

        public List<Review> Reviews { get; set; } = [];

        public decimal AverageRating
        {
            get
            {
                if (Reviews == null || Reviews.Count == 0)
                    return 0;
                return Reviews.Average(r => r.Rating);
            }
        }

        public string RatingImageName
        {
            get
            {
                if (Reviews == null || Reviews.Count == 0)
                    return "rating-0.png"; 
      
                int roundedRating = (int)(Math.Round(AverageRating / 5) * 5);

                roundedRating = Math.Max(0, Math.Min(50, roundedRating));

                return $"rating-{roundedRating:D2}.png"; 
            }
        }

        public int ReviewCount
        {
            get
            {
                return Reviews?.Count ?? 0;
            }
        }
    }
}