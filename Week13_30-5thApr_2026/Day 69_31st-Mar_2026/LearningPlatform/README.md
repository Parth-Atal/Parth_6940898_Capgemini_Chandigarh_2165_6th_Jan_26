# 🎓 Learnify — Online Learning Platform

A full-stack online learning platform built with **ASP.NET Core 9 Web API** and **Vanilla HTML/CSS/JS**, inspired by Udemy.

---

## 📁 Project Structure

```
LearningPlatform/
├── LearningPlatform.sln
│
├── LearningPlatform.API/              ← ASP.NET Core 9 Web API
│   ├── Controllers/
│   │   ├── AuthController.cs          ← Register / Login → JWT
│   │   ├── CoursesController.cs       ← CRUD + Caching + Role-based
│   │   ├── EnrollmentController.cs    ← Enroll / Unenroll
│   │   └── UsersController.cs         ← Profile management
│   ├── Data/
│   │   └── AppDbContext.cs            ← EF Core + Fluent API
│   ├── DTOs/
│   │   └── Dtos.cs                    ← All DTOs (CourseDto, LessonDto, UserDto…)
│   ├── Mappings/
│   │   └── MappingProfile.cs          ← AutoMapper profiles
│   ├── Middleware/
│   │   └── ErrorHandlingMiddleware.cs ← Global exception handler
│   ├── Migrations/                    ← EF Core migration (pre-generated)
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Profile.cs
│   │   ├── Course.cs
│   │   ├── Lesson.cs
│   │   └── Enrollment.cs
│   ├── Services/
│   │   └── JwtService.cs             ← JWT token generation
│   ├── appsettings.json
│   └── Program.cs                    ← Full middleware pipeline
│
└── LearningPlatform.Frontend/         ← Vanilla HTML/CSS/JS
    ├── login.html                     ← Login page
    ├── register.html                  ← Registration page
    ├── courses.html                   ← Course listing + search/filter
    ├── create-course.html             ← Create course (Instructor/Admin)
    ├── my-enrollments.html            ← My enrolled courses (Student)
    ├── css/
    │   └── styles.css                 ← Full responsive stylesheet
    └── js/
        ├── api.js                     ← Centralized fetch API client
        └── auth.js                    ← Auth helpers + validation utilities
```

---

## 🗄️ Database Design

### Entities & Relationships

| Relationship | Description |
|---|---|
| **One-to-One** | User ↔ Profile |
| **One-to-Many** | Course → Lessons |
| **Many-to-Many** | User ↔ Course (via Enrollment join table) |

All relationships are configured via **Fluent API** in `AppDbContext.cs`.

---

## 🚀 Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or LocalDB)
- [VS Code](https://code.visualstudio.com/) or Visual Studio 2022

---

### Step 1 — Configure Database

Edit `LearningPlatform.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LearningPlatformDb;Trusted_Connection=True;"
}
```

For SQL Server Express use:
```
Server=.\\SQLEXPRESS;Database=LearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;
```

---

### Step 2 — Restore & Run the API

```bash
cd LearningPlatform/LearningPlatform.API
dotnet restore
dotnet ef database update    # Applies migration + seed data
dotnet run
```

The API will start at:
- `https://localhost:7001`
- `http://localhost:5001`
- Swagger UI: `https://localhost:7001/swagger`

---

### Step 3 — Configure Frontend

Edit `LearningPlatform.Frontend/js/api.js` line 6:

```javascript
const API_BASE = 'https://localhost:7001';  // Must match your API URL
```

---

### Step 4 — Open Frontend

Open with **Live Server** (VS Code extension) or any HTTP server:

```bash
cd LearningPlatform.Frontend
npx serve .        # or use VS Code Live Server
```

Open `http://localhost:5500/login.html`

---

## 🔐 Authentication

JWT tokens include:
- `sub` — User ID
- `unique_name` — Username
- `email` — Email
- `role` — Student / Instructor / Admin

### Demo Accounts (seeded)

| Role | Email | Password |
|---|---|---|
| Admin | admin@learnify.com | Admin@123 |
| Instructor | instructor@learnify.com | Admin@123 |

Register new accounts via `/api/auth/register` or the Register page.

---

## 📡 API Endpoints

### Auth
| Method | Route | Access |
|---|---|---|
| POST | /api/auth/register | Public |
| POST | /api/auth/login | Public |

### Courses
| Method | Route | Access |
|---|---|---|
| GET | /api/v1/courses | Public (cached 5 min) |
| GET | /api/v1/courses/{id} | Public |
| GET | /api/v1/courses/category/{name} | Public |
| GET | /api/v1/courses/my | Instructor, Admin |
| POST | /api/v1/courses | Instructor, Admin |
| PUT | /api/v1/courses/{id} | Instructor (own), Admin |
| DELETE | /api/v1/courses/{id} | Admin |
| POST | /api/v1/courses/{id}/lessons | Instructor, Admin |

### Enrollments
| Method | Route | Access |
|---|---|---|
| POST | /api/v1/enroll | Student, Admin |
| GET | /api/v1/enroll/my | Authenticated |
| DELETE | /api/v1/enroll/{courseId} | Authenticated |
| GET | /api/v1/enroll/all | Admin |

### Users
| Method | Route | Access |
|---|---|---|
| GET | /api/v1/users/me | Authenticated |
| PUT | /api/v1/users/profile | Authenticated |
| GET | /api/v1/users | Admin |
| DELETE | /api/v1/users/{id} | Admin |

---

## 🏗️ Architecture Highlights

### DTO + AutoMapper
- All API inputs/outputs go through DTOs (prevents over-posting)
- `MappingProfile.cs` handles all entity ↔ DTO conversions

### Caching (IMemoryCache)
- `GET /api/v1/courses` is cached for **5 minutes**
- Cache is invalidated automatically on course Create / Update / Delete

### Server-side Validation
- DataAnnotations on DTOs (`[Required]`, `[EmailAddress]`, `[MinLength]`)
- Manual checks return `{ "error": "..." }` shaped responses

### Client-side Validation
- Empty field checks before any API call
- Email format regex validation
- Minimum password length enforcement

### Role-based Authorization
```csharp
[Authorize(Roles = "Instructor,Admin")]  // Create course
[Authorize(Roles = "Student,Admin")]     // Enroll
[Authorize(Roles = "Admin")]             // Delete / admin routes
```

---

## 🔧 EF Core Commands

```bash
# Add a new migration
dotnet ef migrations add <MigrationName>

# Apply to database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Drop database
dotnet ef database drop
```

---

## 📦 NuGet Packages Used

| Package | Purpose |
|---|---|
| Microsoft.EntityFrameworkCore.SqlServer | ORM + SQL Server driver |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT authentication |
| AutoMapper.Extensions.Microsoft.DependencyInjection | Object mapping |
| BCrypt.Net-Next | Secure password hashing |
| Swashbuckle.AspNetCore | Swagger / OpenAPI docs |
| System.IdentityModel.Tokens.Jwt | JWT token generation |

---

## 🎨 Frontend Features

- Responsive design (mobile + desktop)
- Client-side JWT storage (localStorage)
- Role-aware navigation (Instructor sees "Create Course")
- Course search + category filter
- Enroll / Unenroll with real-time feedback
- Auto-redirect if session expired

---

## 📝 Case Study Coverage

| Requirement | Status |
|---|---|
| EF Core + SQL Server | ✅ |
| One-to-One (User↔Profile) | ✅ |
| One-to-Many (Course→Lessons) | ✅ |
| Many-to-Many (User↔Course) | ✅ |
| Fluent API configuration | ✅ |
| JWT Authentication | ✅ |
| Register + Login endpoints | ✅ |
| Token with Username + Role | ✅ |
| Attribute Routing (/api/v1/...) | ✅ |
| All required routes | ✅ |
| Role-based Authorization | ✅ |
| [Authorize(Roles = "...")] | ✅ |
| DTO + AutoMapper | ✅ |
| IMemoryCache (5 min) | ✅ |
| Server-side validation | ✅ |
| Proper error responses | ✅ |
| Frontend Login page | ✅ |
| Frontend Course List page | ✅ |
| Frontend Create Course page | ✅ |
| Client-side validation | ✅ |
| Fetch API + JWT in localStorage | ✅ |
