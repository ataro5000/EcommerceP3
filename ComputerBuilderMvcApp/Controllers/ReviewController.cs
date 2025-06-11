using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace ComputerBuilderMvcApp.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _reviewsFilePath;

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

        private void SaveReviewsToFile(List<Review> reviews)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(reviews, options);
            System.IO.File.WriteAllText(_reviewsFilePath, json);
        }

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