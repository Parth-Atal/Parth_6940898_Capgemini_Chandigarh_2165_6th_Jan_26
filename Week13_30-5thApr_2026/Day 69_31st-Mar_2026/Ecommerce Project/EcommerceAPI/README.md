# EcommerceAPI — .NET 9 Case Study

A fully working E-Commerce REST API built with **ASP.NET Core 9**, **Entity Framework Core**, and **JWT Authentication**.

---

## 📁 Project Structure

```
EcommerceAPI.sln
│
├── EcommerceAPI/                        ← Main Web API
│   ├── Controllers/
│   │   ├── AuthController.cs            ← POST /api/auth/login
│   │   └── ProductsController.cs        ← CRUD /api/v1/products
│   ├── Data/
│   │   └── AppDbContext.cs              ← EF Core DbContext
│   ├── Models/
│   │   └── Product.cs                   ← Product entity
│   ├── Program.cs                       ← App bootstrap + JWT + Swagger
│   ├── appsettings.json                 ← JWT config + connection string
│   └── EcommerceAPI.csproj
│
└── EcommerceAPI.Tests/                  ← xUnit Test Project
    ├── ProductTests.cs                  ← 10 unit tests for ProductsController
    ├── AuthTests.cs                     ← 4 unit tests for AuthController
    └── EcommerceAPI.Tests.csproj
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- SQL Server (optional — falls back to InMemory DB automatically)

### Run the API

```bash
cd EcommerceAPI
dotnet run
```

Swagger UI opens at: **https://localhost:{port}/swagger**

### Run Tests

```bash
cd EcommerceAPI.Tests
dotnet test
```

---

## 🗄️ Database

By default the API uses an **in-memory database** so you can run it with zero configuration.

To use **SQL Server**, set the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EcommerceDB;Trusted_Connection=True;"
}
```

Then run migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🔐 Authentication Flow

All admin endpoints require a **Bearer JWT token**.

### Step 1 — Get a token

```
POST /api/auth/login?username=admin
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expires": "2026-04-02T05:00:00Z"
}
```

### Step 2 — Use the token

Add the header to protected requests:

```
Authorization: Bearer <your-token>
```

Or click **Authorize** in Swagger UI and paste the token.

---

## 📦 API Endpoints

### Auth

| Method | Route              | Auth | Description      |
|--------|--------------------|------|------------------|
| POST   | /api/auth/login    | ❌   | Get JWT token    |

### Products

| Method | Route                          | Auth    | Description              |
|--------|--------------------------------|---------|--------------------------|
| GET    | /api/v1/products               | ❌      | Get all products         |
| GET    | /api/v1/products/{id}          | ❌      | Get product by ID        |
| GET    | /api/v1/products/category/{c}  | ❌      | Filter by category       |
| POST   | /api/v1/products               | ✅ Admin | Create product           |
| PUT    | /api/v1/products/{id}          | ✅ Admin | Update product           |
| DELETE | /api/v1/products/{id}          | ✅ Admin | Delete product           |

---

## 🧪 Unit Tests

| Test Class     | Tests | Coverage                                      |
|----------------|-------|-----------------------------------------------|
| ProductTests   | 10    | GetAll, GetById, GetByCategory, Create, Update, Delete |
| AuthTests      | 4     | Login success, empty username, whitespace, token validity |

All tests use **EF Core InMemory** database — no SQL Server required.

---

## ⚙️ JWT Configuration (`appsettings.json`)

```json
"Jwt": {
  "Key": "THIS_IS_A_SECURE_KEY_1234567890123456",
  "Issuer": "EcommerceAPI",
  "Audience": "EcommerceUsers"
}
```

> ⚠️ **Change the Key before deploying to production!**

---

## 📦 NuGet Packages

### EcommerceAPI
| Package | Version |
|---------|---------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.0 |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.0 |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.0 |
| Microsoft.EntityFrameworkCore.Tools | 9.0.0 |
| Swashbuckle.AspNetCore | 6.6.2 |

### EcommerceAPI.Tests
| Package | Version |
|---------|---------|
| xunit | 2.9.2 |
| Moq | 4.20.72 |
| Microsoft.NET.Test.Sdk | 17.11.1 |
| Microsoft.AspNetCore.Mvc.Testing | 9.0.0 |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.0 |
