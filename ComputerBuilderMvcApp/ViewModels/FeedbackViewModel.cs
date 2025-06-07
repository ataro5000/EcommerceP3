using System.ComponentModel.DataAnnotations;

namespace ComputerBuilderMvcApp.ViewModels
{
    public class FeedbackViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Subject cannot be longer than 100 characters.")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, ErrorMessage = "Message cannot be longer than 1000 characters.")]
        public string Message { get; set; } = string.Empty;

        public string? Name { get; set; } // Optional
    }
}