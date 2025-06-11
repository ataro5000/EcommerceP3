document.addEventListener('DOMContentLoaded', function () {
    const addedToCartElements = document.querySelectorAll(".added-to-cart");

    addedToCartElements.forEach(element => {
        element.style.opacity = "0";
        element.style.transition = "opacity 0.5s ease-in-out";
    });

    const miniCartPreview = document.getElementById('miniCartPreview');
    const toggleMiniCartBtn = document.getElementById('toggleMiniCartBtn');
    const closeMiniCartBtn = document.getElementById('closeMiniCartBtn');

    if (toggleMiniCartBtn) {
        toggleMiniCartBtn.addEventListener('click', function() {
            if (miniCartPreview) {
                const isHidden = miniCartPreview.style.display === 'none' || miniCartPreview.style.display === '';
                miniCartPreview.style.display = isHidden ? 'block' : 'none';
                if (isHidden) {
                    updateCartSummaryDisplay(null); 
                }
            }
        });
    }

    if (closeMiniCartBtn) {
        closeMiniCartBtn.addEventListener('click', function() {
            if (miniCartPreview) {
                miniCartPreview.style.display = 'none';
            }
        });
    }

    document.querySelectorAll(".add-to-cart-button").forEach(button => {
        button.addEventListener("click", function (event) {
            event.preventDefault();

            const productContainer = this.closest(".product-container, .details-item-container"); 
            const addedToCartElement = productContainer ? productContainer.querySelector(".added-to-cart") : null;
            const form = this.closest("form");

            if (addedToCartElement) {
                addedToCartElement.style.opacity = "1";
                setTimeout(() => {
                    addedToCartElement.style.opacity = "0";
                }, 2000);
            }

            const formData = new FormData(form);
            fetch(form.action, {
                method: form.method,
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        const itemNameMatch = data.message.match(/^(.*?) \(x\d+\) added to cart\.$/);
                        const lastAddedItemName = itemNameMatch ? itemNameMatch[1] : 'Item';
                        
                        updateCartSummaryDisplay(lastAddedItemName);

                    } else {
                        console.error('Error adding item to cart:', data.message);
                        updateCartSummaryDisplay(null); 
                    }
                })
                .catch(error => {
                    console.error('Error during AJAX request:', error);
                    updateCartSummaryDisplay(null); 
                });
        });
    });

    updateCartSummaryDisplay(null); 
});

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
                    miniCartLastItemEl.textContent = 'N/A'; 
                }
      
            }
        })
        .catch(error => {
            console.error('Error fetching or processing cart summary:', error);
        });
}