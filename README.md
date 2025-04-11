# Investment Portfolio API (.NET 8)

This is a full-featured Web API project built with **.NET 8**, **Entity Framework Core**, **MS SQL Server**, and **MongoDB**.  
It allows users to manage currency-based investment portfolios, upload assets via CSV, convert currencies using real exchange rates from the internet, and log transactions.

---

## Architectural Overview

The project follows a clean, modular, and scalable architecture using the following technologies:

- ASP.NET Core Web API (.NET 8) – for exposing RESTful endpoints
- Entity Framework Core 8 – for data access and SQL Server integration
- MongoDB – for logging actions, errors, and file uploads
- JWT Authentication – for secure login with Admin/User role separation
- Swagger (OpenAPI) – for easy testing and documentation
- ExchangeRate API – for fetching real-time currency exchange data

### Folder Structure

InvestmentPortfolioAPI/
- Controllers/ # Handles API endpoints
- Data/ # ApplicationDbContext + Factory
- Migrations/ # EF Core database migrations
- Models/ # EF + DTO models, enums
  - Dto/ # DTOs for requests/responses
  - Mongo/ # LogEntry, MongoDB models
- Services/ # Logic layer: exchange fetch, Mongo logger
- appsettings.json # General settings (safe, no secrets)
- appsettings.Development.json # Local DB + JWT config
- Program.cs # App startup & service config



## Authentication & Roles

- Login is JWT-based (no signup)
- Users receive a JWT to access protected endpoints via Swagger or client
  
- User Role:
  - Can manage their own assets and portfolios
  - Can convert currencies and evaluate portfolio
- Admin Role:
  - Can view/manage all portfolios
  - Can manually create or update exchange rates
  - Can add/edit asset types


## External API Usage (Documented in Code)

The system fetches real exchange rates from [ExchangeRate Host](https://exchangerate.host/), a free, no-auth public API.

### API Endpoint Used:

GET https://api.exchangerate.host/latest?base=USD
This endpoint returns exchange rates from a base currency to others. (I had a problem about posting the results to db but it calls from this)


## Running the Project Locally

### Requirements
- .NET 8 SDK
- SQL Server (local or cloud)
- MongoDB (running locally on mongodb://localhost:27017)
- Visual Studio or VS Code

## Setup Instructions

1. Clone the project

git clone https://github.com/Enver4/InvestmentPortfolioAPI
cd InvestmentPortfolioAPI

2. Configure your settings

Update appsettings.Development.json with:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=InvestmentDb;Trusted_Connection=True;MultipleActiveResultSets=true"
},
  "MongoSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "InvestmentLogsDb"
},
  "Jwt": {
    "Key": "ThisIsASecureJwtKey1234567890!!@#", 
    "Issuer": "InvestmentAPI",
    "Audience": "InvestmentAPIUsers"
}
}

3. Apply EF Core Migrations

dotnet ef database update

4. Run the API

dotnet run 
or you can run Program.cs from run button on top

5. Open Swagger at:
http://localhost:{PORT}/swagger

Use /api/auth/login to get a token (id: admin, pass: admin123 e.g) > click Authorize > test secured endpoints.
   


