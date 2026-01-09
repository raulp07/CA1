# Walkthrough for CA1

## 1. Setup & Run
1. Navigate to the WebAPI folder:
   ```powershell
   cd CA1.WebAPI
   ```
2. Run the application:
   ```powershell
   dotnet run
   ```
   *The application will create the database `CA1.db` automatically.*
   
   Base URL: `http://localhost:5220`

## 2. Testing Logic (Walkthrough)

You can use Postman, curl, or standard PowerShell commands `Invoke-RestMethod`.

### Step A: Register (Create User)
This creates a user in the SQLite DB using **Dapper** (via `UserRepository`).
```powershell
$body = @{ username = "demo_user"; password = "password123"; role = "Admin" } | ConvertTo-Json
$response = Invoke-RestMethod -Uri "http://localhost:5220/api/auth/register" -Method Post -Body $body -ContentType "application/json"
echo $response
```
*Save the `token` from the response.*

### Step B: Login (Authenticate)
Verifies credentials referencing the DB.
```powershell
$body = @{ username = "demo_user"; password = "password123" } | ConvertTo-Json
$loginResponse = Invoke-RestMethod -Uri "http://localhost:5220/api/auth/login" -Method Post -Body $body -ContentType "application/json"
$token = $loginResponse.token
echo "Token received: $token"
```

### Step C: Create Product (Protected Route)
Uses **Entity Framework Core** to save to DB. Requires the JWT token from Step B.
```powershell
$headers = @{ Authorization = "Bearer $token" }
$product = @{ name = "High-End Laptop"; description = "Powerful dev machine"; price = 2500 } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5220/api/products" -Method Post -Body $product -ContentType "application/json" -Headers $headers
```

### Step D: Get Products (Protected Route)
Retrieves data using EF Core.
```powershell
Invoke-RestMethod -Uri "http://localhost:5220/api/products" -Method Get -Headers $headers
```

## 3. Architecture Overview
- **Domain**: Contains `User`, `Product` entities. No dependencies.
- **Application**: Contains Interfaces (`IUserRepository`, `IProductService`) and Business Logic (`AuthService`).
- **Infrastructure**: Implements Interfaces.
  - `UserRepository`: Uses **Dapper** (Micro-ORM) for SQL queries.
  - `ProductRepository`: Uses **EF Core** for ORM capabilities.
  - `AppDbContext`: EF Core Context.
- **WebAPI**: Controllers and Dependency Injection configuration.

This setup demonstrates how to mix EF Core and Dapper within the same Clean Architecture solution, all secured by JWT.
