using System.ComponentModel.DataAnnotations;

namespace ComputerBuilderMvcApp.ViewModels
{
    public class FeedbackViewModel
    {

        public string Email { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public string? Name { get; set; } 
    }
}