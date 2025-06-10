document.addEventListener('DOMContentLoaded', function () {
    const addedToCartElements = document.querySelectorAll(".added-to-cart");

    addedToCartElements.forEach(element => {
        element.style.opacity = "0"; 
        element.style.transition = "opacity 0.5s ease-in-out"; 
    });

    document.querySelectorAll(".add-to-cart-button").forEach(button => {
        button.addEventListener("click", function (event) {
            event.preventDefault(); 

            const productContainer = this.closest(".product-container");
            const addedToCartElement = productContainer.querySelector(".added-to-cart");
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
                        updateCartItemCountInLayout(); 

                    } else {
                        console.error('Error adding item to cart:', data.message);
                    }
                })
                .catch(error => {
                    console.error('Error during AJAX request:', error);
                });
        });
    });

    updateCartItemCountInLayout();
});

function updateCartItemCountInLayout() {
    fetch('/Cart/GetCartItemCount')
        .then(response => response.json())
        .then(data => {
            const cartBadge = document.getElementById('cartItemCountBadge');
            if (cartBadge) {
                cartBadge.textContent = data.itemCount || 0;
            }
        })
        .catch(error => {
            console.error('Error fetching or processing cart item count:', error);
        });
}