
# Tertiary School Navigator (TSN)

TSN is a web API designed to facilitate the search for tertiary schools in Ghana. The API allows users to search for schools by region, district, and school name. It provides comprehensive information about each school, including the courses they offer, location, contact information, and the programs they offer. TSN covers a wide range of tertiary institutions in Ghana, such as universities, polytechnics, colleges of education, and nursing training schools.

## Features

- Search for schools by region, district, and name.
- Detailed information on courses offered, location, contact details, and programs.
- Comprehensive coverage of tertiary institutions in Ghana.

## Technologies Used

- **C#**: The primary programming language used for backend development.
- **.Net Core 8**: The framework used for building the web API.
- **Entity Framework Core**: The ORM (Object-Relational Mapping) framework for data access.
- **SQL Server**: The database management system used for storing school data.
- **Swagger**: Used for API documentation and testing.
- **Postman**: Utilized for API testing.
- **FluentValidation**: For validating input data.

## Installation

Follow these steps to set up the project locally:

1. **Clone the Repository**

   First, clone the TSN repository to your local machine.

   ```bash
   git clone <repository_url>
   ```

   Replace `<repository_url>` with the actual URL of the TSN repository.

2. **Navigate to the Project Folder**

   Change your current directory to the TSN project folder.

   ```bash
   cd TertiarySchoolNavigator
   ```

3. **Run the Project**

   Start the project using the .NET Core CLI.

   ```bash
   dotnet run
   ```

## Usage

After setting up the project, you can access the API documentation through Swagger at `http://localhost:<port>/swagger`. Replace `<port>` with the port number on which your application is running.

