// This script handles various client-side interactions for the Computer Builder application.
// It manages the display of "Added to Cart" notifications, toggles a mini-cart preview,
// processes "Add to Cart" actions via AJAX, updates the cart item count badge and mini-cart summary,
// and implements interactive star rating functionality for product reviews.
// All interactions are initialized after the DOM content is fully loaded.

document.addEventListener('DOMContentLoaded', function () {
    // Initializes and controls the "Added to Cart" visual feedback elements.
    // These elements fade in and out when an item is added to the cart.
    const addedToCartElements = document.querySelectorAll(".added-to-cart");

    addedToCartElements.forEach(element => {
        element.style.opacity = "0"; // Start fully transparent
        element.style.transition = "opacity 0.5s ease-in-out"; // Smooth transition for opacity
    });

    const miniCartPreview = document.getElementById('miniCartPreview');
    const toggleMiniCartBtn = document.getElementById('toggleMiniCartBtn');
    const closeMiniCartBtn = document.getElementById('closeMiniCartBtn');

    // Attaches an event listener to the mini-cart toggle button.
    // Shows or hides the mini-cart preview and updates its content when shown.
    if (toggleMiniCartBtn) {
        toggleMiniCartBtn.addEventListener('click', function() {
            if (miniCartPreview) {
                const isHidden = miniCartPreview.style.display === 'none' || miniCartPreview.style.display === '';
                miniCartPreview.style.display = isHidden ? 'block' : 'none';
                if (isHidden) {
                    // Update the cart summary display when opening the mini cart
                    updateCartSummaryDisplay(null); 
                }
            }
        });
    }

    // Attaches an event listener to the mini-cart close button.
    // Hides the mini-cart preview when clicked.
    if (closeMiniCartBtn) {
        closeMiniCartBtn.addEventListener('click', function() {
            if (miniCartPreview) {
                miniCartPreview.style.display = 'none';
            }
        });
    }

    // Attaches event listeners to all "Add to Cart" buttons.
    // Handles adding items to the cart asynchronously using Fetch API.
    // Shows an "Added to Cart" notification and updates the cart summary upon successful addition.
    document.querySelectorAll(".add-to-cart-button").forEach(button => {
        button.addEventListener("click", function (event) {
            event.preventDefault(); // Prevent default form submission

            const productContainer = this.closest(".product-container, .details-item-container"); 
            const addedToCartElement = productContainer ? productContainer.querySelector(".added-to-cart") : null;
            const form = this.closest("form");

            // Show "Added to Cart" visual feedback
            if (addedToCartElement) {
                addedToCartElement.style.opacity = "1";
                setTimeout(() => {
                    addedToCartElement.style.opacity = "0";
                }, 2000); // Hide after 2 seconds
            }

            const formData = new FormData(form);
            fetch(form.action, {
                method: form.method,
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Extract item name from the success message for display in mini-cart
                        const itemNameMatch = data.message.match(/^(.*?) \(x\d+\) added to cart\.$/);
                        const lastAddedItemName = itemNameMatch ? itemNameMatch[1] : 'Item';
                        
                        // Update cart summary display with the name of the last added item
                        updateCartSummaryDisplay(lastAddedItemName);

                    } else {
                        console.error('Error adding item to cart:', data.message);
                        // Update cart summary display even if there's an error (e.g., to refresh count)
                        updateCartSummaryDisplay(null); 
                    }
                })
                .catch(error => {
                    console.error('Error during AJAX request:', error);
                     // Update cart summary display even if there's a network error
                    updateCartSummaryDisplay(null); 
                });
        });
    });

    // Manages the interactive star rating input for reviews.
    // Allows users to select a rating by clicking on stars, and updates a hidden input field.
    const starContainer = document.getElementById('starRatingContainer');
    const ratingInput = document.getElementById('Rating'); // Hidden input for the actual rating value
    const stars = starContainer ? Array.from(starContainer.getElementsByClassName('rating-star-input')) : [];
    // Note: The @Url.Content paths will be resolved by Razor if this script is in a .cshtml file or correctly linked.
    // If this is a static .js file, these paths need to be absolute or relative to the HTML file.
    const emptyStarSrc = '/images/ratings/rating-0star.png'; // Assuming this path is correct from wwwroot
    const filledStarSrc = '/images/ratings/rating-1star.png'; // Assuming this path is correct from wwwroot

    let initialSubmittedRating = ratingInput ? parseInt(ratingInput.value) : 0;

    // Validate and sanitize the initial rating value from the hidden input.
    if (isNaN(initialSubmittedRating) || initialSubmittedRating < 0 || initialSubmittedRating > 50 || initialSubmittedRating % 10 !== 0) {
        initialSubmittedRating = 0;
        if (ratingInput) ratingInput.value = "0"; 
    }
    // displayRating is the 0-5 star count for UI logic (e.g., 30 becomes 3 stars)
    let displayRating = initialSubmittedRating / 10;

    // Updates the visual appearance of the stars based on the provided rating.
    // `ratingToDisplay` is a value from 0 to 5, representing the number of filled stars.
    function updateStarsDisplay(ratingToDisplay) { 
        stars.forEach(star => {
            const starNumericValue = parseInt(star.dataset.value); // 1-5 (value of the star element itself)
            if (starNumericValue <= ratingToDisplay) {
                star.src = filledStarSrc;
            } else {
                star.src = emptyStarSrc;
            }
        });
    }

    if (starContainer && ratingInput) {
        updateStarsDisplay(displayRating); // Set initial star display based on current rating

        // Handles mouseover event on stars to provide hover feedback.
        starContainer.addEventListener('mouseover', function (e) {
            if (e.target.classList.contains('rating-star-input')) {
                const hoverValue = parseInt(e.target.dataset.value); // 1-5
                updateStarsDisplay(hoverValue); 
            }
        });

        // Handles mouseout event on stars to revert to the selected rating.
        starContainer.addEventListener('mouseout', function () {
            // Revert to the currently selected displayRating
            updateStarsDisplay(displayRating);
        });

        // Handles click event on stars to set the rating.
        starContainer.addEventListener('click', function (e) {
            if (e.target.classList.contains('rating-star-input')) {
                displayRating = parseInt(e.target.dataset.value); // Update displayRating (1-5 stars)
                ratingInput.value = displayRating * 10; // Set hidden input to 10-50 for submission
                updateStarsDisplay(displayRating); // Update visual stars
            }
        });
    }

    // Initial call to update cart summary display when the page loads.
    updateCartSummaryDisplay(null); 
});

// Fetches the current cart item count and total price from the server.
// Updates the cart badge in the header and the mini-cart preview content.
// `lastAddedItemName` is an optional parameter to display the name of the most recently added item.
function updateCartSummaryDisplay(lastAddedItemName) {
    fetch('/Cart/GetCartItemCount') 
        .then(response => response.json())
        .then(data => {
            const cartBadge = document.getElementById('cartItemCountBadge');
            if (cartBadge) {
                cartBadge.textContent = data.itemCount || 0;
            }

            const miniCartLastItemEl = document.getElementById('miniCartLastItem');
            const miniCartTotalPriceEl = document.getElementById('miniCartTotalPrice');

            if (miniCartTotalPriceEl) {
                miniCartTotalPriceEl.textContent = data.totalCartPrice || '$0.00';
            }

            if (miniCartLastItemEl) {
                if (lastAddedItemName) {
                    miniCartLastItemEl.textContent = lastAddedItemName;
                } else if (data.itemCount === 0) {
                    // If cart is empty and no specific item was just added, show N/A
                    miniCartLastItemEl.textContent = 'N/A'; 
                }
                // If an item was previously displayed and now cart is not empty but no new item added,
                // it will retain the last item's name until a new item is added or cart becomes empty.
            }
        })
        .catch(error => {
            console.error('Error fetching or processing cart summary:', error);
            // Optionally, handle UI update on error, e.g., show 'Error' in cart summary
        });
}