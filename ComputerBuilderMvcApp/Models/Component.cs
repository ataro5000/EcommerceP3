// This file defines the Component class, which represents a computer component.
// It includes properties for the component's details, such as ID, type, name, price, specifications, and image.
// It also manages a list of reviews for the component and calculates the average rating and review count.
namespace ComputerBuilderMvcApp.Models
{
    public class Component
    {
        // Gets or sets the unique identifier for the component.
        public string? Id { get; set; }
        // Gets or sets the type or category of the component (e.g., CPU, GPU, RAM).
        public string? Type { get; set; }
        // Gets or sets the name of the component.
        public string? Name { get; set; }
        // Gets or sets the price of the component in cents.
        public decimal PriceCents { get; set; }
        // Gets or sets the specifications or description of the component.
        public string? Spec { get; set; }
        // Gets or sets the image URL or path for the component.
        public string? Image { get; set; }
        // Gets or sets the list of reviews associated with this component.
        public List<Review> Reviews { get; set; } = [];
        // Calculates the average rating for the component based on its reviews.
        // Returns 0 if there are no reviews.
        public decimal AverageRating
        {
            get
            {
                if (Reviews == null || Reviews.Count == 0)
                    return 0;
                return Reviews.Average(r => r.Rating);
            }
        }
        // Determines the filename of the rating image based on the average rating.
        // Ratings are rounded to the nearest 5-point increment (0-50).
        // Returns "rating-0.png" if there are no reviews.
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
        // Gets the total number of reviews for the component.
        public int ReviewCount
        {
            get
            {
                return Reviews?.Count ?? 0;
            }
        }
    }
}