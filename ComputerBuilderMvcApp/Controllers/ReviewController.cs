// This file defines the ReviewController class, which is responsible for handling component reviews.
// It allows users to add new reviews for components and saves them to a JSON file.
using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Text.Json;


namespace ComputerBuilderMvcApp.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _reviewsFilePath;

        // Constructor for the ReviewController.
        // Initializes the web host environment and sets the path for the reviews JSON file.
        // Ensures the Data directory exists.
        public ReviewController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

            string dataDirPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data");
            if (!Directory.Exists(dataDirPath))
            {
                Directory.CreateDirectory(dataDirPath); // Ensure the Data directory exists
            }
            _reviewsFilePath = Path.Combine(dataDirPath, "reviews.json");
        }

        // Loads reviews from the reviews.json file.
        // Returns a list of Review objects. If the file doesn't exist or is empty, an empty list is returned.
        private List<Review> LoadReviewsFromFile()
        {
            if (!System.IO.File.Exists(_reviewsFilePath))
            {
                return [];
            }

            var json = System.IO.File.ReadAllText(_reviewsFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return [];
            }
            return JsonSerializer.Deserialize<List<Review>>(json) ?? new List<Review>();
        }

        // Saves a list of reviews to the reviews.json file.
        // Serializes the list of Review objects to JSON and writes it to the file.
        private void SaveReviewsToFile(List<Review> reviews)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(reviews, options);
            System.IO.File.WriteAllText(_reviewsFilePath, json);
        }

        // Handles the submission of a new component review.
        // Validates the review data. If valid, it adds the new review to the list, saves it,
        // and redirects to the component's detail page.
        // If invalid, it stores error messages and submitted data in TempData and redirects
        // back to the component's detail page to display errors.
        [HttpPost]
        public IActionResult AddComponentReview(Review reviewViewModel)
        {
  
            if (string.IsNullOrWhiteSpace(reviewViewModel.ItemId)) ModelState.AddModelError("ItemId", "Item ID is required.");
            
            if (reviewViewModel.Rating < 0 || reviewViewModel.Rating > 50 || reviewViewModel.Rating % 5 != 0)
            {
                ModelState.AddModelError("Rating", "Rating must be between 0 and 50, in 5-point increments.");
            }

            if (string.IsNullOrWhiteSpace(reviewViewModel.Comments)) ModelState.AddModelError("Comments", "Comments are required.");


            if (ModelState.IsValid)
            {
                var reviews = LoadReviewsFromFile();

                var newReview = new Review
                {
                    ID = reviews.Count != 0 ? reviews.Max(r => r.ID) + 1 : 1,
                    ItemId = reviewViewModel.ItemId,
                    Rating = reviewViewModel.Rating,
                    Comments = reviewViewModel.Comments,
                    CustomerName = string.IsNullOrWhiteSpace(reviewViewModel.CustomerName) ? "Anonymous" : reviewViewModel.CustomerName,
                    ReviewDate = DateTime.UtcNow
                };

                reviews.Add(newReview);
                SaveReviewsToFile(reviews);

                // Redirect back to the component's detail page
                return RedirectToAction("Details", "Components", new { id = newReview.ItemId });
            }

            var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["ReviewError"] = string.Join("; ", errorList);
            TempData["SubmittedReviewData_CustomerName"] = reviewViewModel.CustomerName;
            TempData["SubmittedReviewData_Rating"] = reviewViewModel.Rating;
            TempData["SubmittedReviewData_Comments"] = reviewViewModel.Comments;

            return RedirectToAction("Details", "Components", new { id = reviewViewModel.ItemId });
        }
    }
}