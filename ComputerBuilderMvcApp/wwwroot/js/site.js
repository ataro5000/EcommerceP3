
document.addEventListener('DOMContentLoaded', function () {
    const filterButton = document.getElementById('filterDropdownButton');
    const filterMenu = document.getElementById('filterDropdownMenu');

    if (filterButton && filterMenu) {
        filterButton.addEventListener('click', function (event) {
            event.stopPropagation(); // Prevent click from immediately closing if bubbling to document
            filterMenu.classList.toggle('show');
        });

        // Close the dropdown if the user clicks outside of it
        document.addEventListener('click', function (event) {
            if (filterMenu.classList.contains('show') && 
                !filterButton.contains(event.target) && 
                !filterMenu.contains(event.target)) {
                filterMenu.classList.remove('show');
            }
        });
    }

    updateCartItemCountInLayout();
});

function updateCartItemCountInLayout() {
    fetch('/Cart/GetCartItemCount')
        .then(response => {
            return response.json();
        })
        .then(data => {
            const cartBadge = document.getElementById('cartItemCountBadge');
            if (cartBadge) {
                cartBadge.textContent = data.itemCount || 0;
            }
        })
        .catch(error => {
            console.error('Error fetching or processing cart item count:', error);
            const cartBadge = document.getElementById('cartItemCountBadge');

        });
}
