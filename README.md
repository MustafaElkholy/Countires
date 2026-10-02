# Countries and Cities API

An ASP.NET Core Web API for managing countries and their cities.

The project was built as a CRUD assignment. It includes request validation, pagination, filtering, soft deletion, and centralized exception handling. Swagger UI is included for exploring and testing the endpoints.

## Features

- Create, retrieve, update, and delete countries.
- Create, retrieve, update, and delete cities.
- Search countries by name.
- Search cities by name and country ID.
- Retrieve cities belonging to a country with pagination.
- Validate requests before saving changes.
- Prevent deleting countries that still have active cities.

## Tools used

| Tool | Purpose |
|---|---|
| .NET 10 and C# | Application runtime and language |
| ASP.NET Core | Controllers, routing, and dependency injection |
| Entity Framework Core | Database queries, change tracking, and migrations |
| SQL Server | Data storage |
| FluentValidation | Request validation |
| Swashbuckle / Swagger UI | API documentation and interactive testing |

## Project structure

The solution follows a layered structure inspired by Clean Architecture:

```text
Countires/
├── Countires.API/
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
├── Countries.Application/
│   ├── DTOs/
│   ├── Exceptions/
│   ├── Extensions/
│   ├── Interfaces/Services/
│   └── Validators/
├── Countries.Domain/
│   └── Entities/
└── Countries.Infrastructure/
    ├── Data/
    │   ├── Configurations/
    │   └── Migrations/
    ├── Extensions/
    ├── Seeder/
    └── Services/
```

### Domain

Contains the `Country` and `City` entities and their relationship. This project has no dependencies on the API or database implementation.

A country can have multiple cities, and each city belongs to one country.

### Application

Contains the request and response DTOs, validation rules, service interfaces, and application exceptions.

Controllers depend on the public service interfaces rather than concrete implementations.

### Infrastructure

Contains the EF Core DbContext, entity configurations, migrations, seed data, and service implementations.

For this CRUD project, services use `ApplicationDbContext` directly.
We didn't use a repository layer or unit of work because EF Core already provides database access and change tracking through `DbSet`, with `SaveChangesAsync` acting as the unit-of-work boundary.

The implementations live in Infrastructure because they depend on EF Core and the DbContext. Application keeps the service contracts.

### API

Contains the controllers, exception middleware, and application startup configuration.

Controllers accept DTOs, call services, and wrap successful results in `ApiResponse<T>`. Validation and business checks happen in the services.

## How requests are handled

A typical request follows this flow:

```text
Controller → Service → DbContext → SQL Server
```

The service validates the request, checks the relevant business rules, and performs the database operation.

If an operation fails, the exception middleware translates service exceptions into HTTP responses:

| Failure | Status |
|---|---|
| Invalid request values | 400 Bad Request |
| Resource not found | 404 Not Found |
| Duplicate record or blocked deletion | 409 Conflict |
| Unexpected server error | 500 Internal Server Error |

Unexpected errors => the client receives a general error message.

## Running locally

### Requirements

- .NET 10 SDK.
- A running SQL Server instance.
- SQL Server LocalDB is also suitable for local development on Windows.
- Git.

### 1. Clone the repository

Replace the placeholders with the repository URL and the resulting directory name.

```bash
git clone <repository-url>
cd <repository-directory>
```

Run the remaining commands from the repository root.

### 2. Configure the database

Set your connection string in:

```text
Countires/Countires.API/appsettings.Development.json
```

Example using SQL Server LocalDB and Windows authentication:

```json
{
  "ConnectionStrings": {
    "CountiresDbConnection": "Server=(localdb)\\MSSQLLocalDB;Database=Countires;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Keep the connection-string key as `CountiresDbConnection`, because that is the name used by the application.

If you use SQL Server or SQL Express instead, replace the server name and authentication settings with those for your instance.

### 3. Restore and build

```bash
dotnet restore Countires/Countires.slnx
dotnet build Countires/Countires.slnx
```

### 4. Trust the local HTTPS certificate

For local HTTPS development:

```bash
dotnet dev-certs https --trust
```

### 5. Start the API

```bash
dotnet run --project Countires/Countires.API/Countries.API.csproj --launch-profile https
```

Open Swagger UI:

```text
https://localhost:7076/swagger
```

When using Visual Studio, select the API as the startup project and run the `https` profile.

### Database initialization

In the Development environment, the application applies existing EF Core migrations automatically and seeds sample countries and cities when the countries table is empty.

There is no separate migration command required for the first local run.

The SQL Server instance must be accessible, and the configured database account must have permission to create the database or apply its migrations.

## API endpoints

### Countries

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/countries/Create` | Create a country |
| GET | `/api/countries/{id}` | Retrieve a country |
| POST | `/api/countries/search` | Search countries with pagination |
| PUT | `/api/countries/{id}` | Update a country |
| DELETE | `/api/countries/{id}` | Soft-delete a country |
| POST | `/api/countries/cities/search` | Retrieve cities by country ID |

### Cities

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/cities/Create` | Create a city |
| GET | `/api/cities/{id}` | Retrieve a city |
| POST | `/api/cities/search` | Search cities with pagination |
| PUT | `/api/cities/{id}` | Update a city |
| DELETE | `/api/cities/{id}` | Soft-delete a city |

Search endpoints use POST, so filters and pagination can be supplied together in a JSON request body.

Send request bodies using `Content-Type: application/json`.


Validation failures include field-level messages in `errors`.

Creation returns HTTP `201`. Successful reads, updates, and deletions return HTTP `200`. Deletion returns a response body containing the confirmation message.

Framework-generated errors, such as malformed JSON, use ASP.NET Core's default error response.

## Validation and business rules

- Country and city names are required and limited to 100 characters.
- Country codes must contain two English letters and are stored in uppercase.
- Country IDs must be positive.
- A city must reference an existing, active country.
- Duplicate active country names and codes are rejected.
- Duplicate active city names within the same country are rejected.
- A country cannot be deleted while it has active cities.

Duplicate checks are performed by the services. They are not enforced by database unique indexes.

## Soft deletion

Delete operations set `IsDeleted` to `true` instead of removing rows from the database.

EF Core global query filters exclude deleted countries and cities from normal queries. Cities belonging to deleted countries are also excluded.

Deleting a country does not automatically delete its cities. Its active cities must be deleted or moved first.

## Testing the API

Use Swagger's **Try it out** option to exercise the endpoints.
