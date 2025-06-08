// JavaScript functionality for the Computer Builder MVC application

document.addEventListener('DOMContentLoaded', function() {
    // Initialize cart from localStorage
    updateCartItemCountInLayout();
       // Function to update cart display
    function updateCartDisplay(itemCount) {
        const cartBadge = document.getElementById('cartItemCountBadge'); // Or some other ID
        if (cartBadge) { // Check if the element exists!
            cartBadge.textContent = itemCount;
        } else {
            console.warn("Cart badge element not found."); // Good for debugging
        }
    }
    updateCartDisplay();
});

function updateCartItemCountInLayout() {
    fetch('/Cart/GetCartItemCount')
        .then(response => {
            if (!response.ok) {
                console.error('Network response was not ok when fetching cart count: ' + response.statusText);
                throw new Error('Network response was not ok: ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {

            const cartBadge = document.getElementById('cartItemCountBadge');
            
            if (cartBadge) {
                cartBadge.textContent = data.itemCount || 0;
            } else {
                console.warn('CRITICAL: Element with ID "cartItemCountBadge" was NOT FOUND in the DOM when trying to update count.');
            }
        })
        .catch(error => {
            console.error('Error fetching or processing cart item count:', error);
            const cartBadge = document.getElementById('cartItemCountBadge');
            if (!cartBadge) {
                console.warn('CRITICAL (in catch block): Element with ID "cartItemCountBadge" also not found.');
            }
        });
}