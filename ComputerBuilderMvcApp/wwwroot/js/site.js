// JavaScript functionality for the Computer Builder MVC application

document.addEventListener('DOMContentLoaded', function() {
    // Initialize cart from localStorage
    let cart = JSON.parse(localStorage.getItem('cart')) || [];

    // Function to update cart display
    function updateCartDisplay() {
        const cartCount = document.getElementById('cart-count');
        const cartTotal = document.getElementById('cart-total');
        const totalPrice = cart.reduce((total, item) => total + item.price, 0);
        
        cartCount.textContent = cart.length;
        cartTotal.textContent = totalPrice.toFixed(2);
    }

    // Function to add item to cart
    function addToCart(item) {
        cart.push(item);
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartDisplay();
    }

    // Event listener for adding components to cart
    document.querySelectorAll('.add-to-cart').forEach(button => {
        button.addEventListener('click', function() {
            const item = {
                id: this.dataset.id,
                name: this.dataset.name,
                price: parseFloat(this.dataset.price)
            };
            addToCart(item);
        });
    });

    // Initial cart display update
    updateCartDisplay();
});