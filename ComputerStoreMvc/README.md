# Computer Store MVC Application

## Overview
This project is an MVC web application for a computer store that allows customers to view and customize computers and their components. The application features a components builder, enabling users to select a computer with a default configuration and customize it by replacing components with compatible options.

## Project Structure
The project is organized into several folders, each serving a specific purpose:

- **Controllers**: Contains the controllers that handle user requests and manage application logic.
  - `HomeController.cs`: Manages the home page.
  - `ComputerController.cs`: Handles requests related to computers.
  - `ComponentController.cs`: Manages component-related requests.

- **Data**: Contains hardcoded JSON files with product information.
  - `computers.json`: List of available computers.
  - `cpus.json`: List of CPU components.
  - `gpus.json`: List of GPU components.
  - `motherboards.json`: List of Motherboard components.
  - `psus.json`: List of PSU components.
  - `ram.json`: List of RAM components.

- **Models**: Contains the data models used in the application.
  - `Computer.cs`: Defines the Computer class.
  - `Component.cs`: Defines the Component class.
  - `Cart.cs`: Manages shopping cart functionality.
  - `Order.cs`: Represents a customer's order.

- **Views**: Contains the Razor views for the application.
  - `Home/Index.cshtml`: Home page view.
  - `Computer/Index.cshtml`: View that lists all computers.
  - `Computer/Detail.cshtml`: View that shows details of a selected computer.
  - `Component/Index.cshtml`: View that lists all available components.
  - `Shared`: Contains shared layout and validation scripts.

- **wwwroot**: Contains static files such as CSS and JavaScript.
  - `css/site.css`: CSS styles for the application.
  - `js/site.js`: JavaScript functionality for the application.

- **Program.cs**: Entry point of the application.
- **Startup.cs**: Configures services and the application's request pipeline.
- **appsettings.json**: Configuration settings for the application.

## Features
- Browse a list of pre-configured computers.
- Customize computers by selecting compatible components.
- View detailed information about each computer and component.
- Hardcoded product information for rapid prototyping.

## Getting Started
To run the application, clone the repository and open it in your preferred IDE. Restore the necessary packages and run the application. You can access the application in your web browser at `http://localhost:5000`.

## Future Enhancements
- Implement a database for dynamic product information.
- Add user authentication for a personalized shopping experience.
- Integrate payment processing for online orders.

## License
This project is licensed under the MIT License.