document.addEventListener("DOMContentLoaded", function() {
    // Fetch and display the list of computers
    fetch('/data/computers.json')
        .then(response => response.json())
        .then(data => {
            const computerList = document.getElementById('computer-list');
            data.forEach(computer => {
                const listItem = document.createElement('li');
                listItem.textContent = computer.Name;
                listItem.onclick = () => showComputerDetails(computer.ID);
                computerList.appendChild(listItem);
            });
        });

    // Function to show computer details and customization options
    function showComputerDetails(computerId) {
        fetch(`/data/computers/${computerId}.json`)
            .then(response => response.json())
            .then(computer => {
                const detailsSection = document.getElementById('computer-details');
                detailsSection.innerHTML = `
                    <h2>${computer.Name}</h2>
                    <p>Total Price: $${computer.TotalPrice.toFixed(2)}</p>
                    <h3>Standard Components:</h3>
                    <ul>
                        ${computer.StandardComponents.map(component => `<li>${component.Type}: ${component.Spec} - $${component.Price.toFixed(2)}</li>`).join('')}
                    </ul>
                    <h3>Customize Components:</h3>
                    <div id="component-options"></div>
                `;
                loadComponentOptions(computerId);
            });
    }

    // Function to load component options for customization
    function loadComponentOptions(computerId) {
        fetch('/data/components.json')
            .then(response => response.json())
            .then(components => {
                const optionsContainer = document.getElementById('component-options');
                components.forEach(component => {
                    const optionDiv = document.createElement('div');
                    optionDiv.innerHTML = `
                        <label>${component.Type}:</label>
                        <select onchange="updateComponentPrice(${computerId}, '${component.Type}', this.value)">
                            ${component.options.map(option => `<option value="${option.Spec}">${option.Spec} - $${option.Price.toFixed(2)}</option>`).join('')}
                        </select>
                    `;
                    optionsContainer.appendChild(optionDiv);
                });
            });
    }

    // Function to update the total price based on selected components
    window.updateComponentPrice = function(computerId, componentType, selectedSpec) {
        // Logic to update the total price based on selected component
        // This would involve fetching the new price and updating the displayed total
    };
});