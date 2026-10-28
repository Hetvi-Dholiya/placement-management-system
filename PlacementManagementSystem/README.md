# College Placement Management System (CPMS)
### ASP.NET Core MVC Mini-Project for B.Tech Computer Science & Engineering

[![.NET Version](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/Database-MySQL%208.0-blue.svg)](https://www.mysql.com/)
[![ORM](https://img.shields.io/badge/ORM-Pomelo%20EF%20Core-orange.svg)](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
[![Frontend](https://img.shields.io/badge/Frontend-Bootstrap%205%20%7C%20Razor-blueviolet.svg)](https://getbootstrap.com/)

---

## 1. Project Overview

The **College Placement Management System (CPMS)** is a full-featured web application designed to automate and streamline campus placement and recruitment activities in engineering colleges. It connects students, department coordinators, placement officers, and corporate recruiters into a unified, responsive platform.

### Academic Requirements Addressed:
1. **MVC Architecture:** Strict separation of Model (Data), View (Razor UI), and Controller (Application logic).
2. **Database Connectivity:** Entity Framework Core Code-First approach with **MySQL** using the `Pomelo.EntityFrameworkCore.MySql` provider.
3. **Complete CRUD Operations:** Demonstrated across Students, Partner Companies, Placement Drives, Applications, and Placements.
4. **Form Handling:** Razor forms with `asp-for`, `asp-action`, `[ValidateAntiForgeryToken]`, and automatic Model Binding.
5. **Model Validation:** Server-side and client-side data annotations (`[Required]`, `[EmailAddress]`, `[Range]`, `[StringLength]`, `[Compare]`).
6. **State Management:**
   - **ASP.NET Core Session:** Retaining authenticated user ID, role, and name across requests.
   - **TempData:** Flash notifications (Success, Error, Warning, Info) across redirects.
7. **Responsive User Interface:** Built with Bootstrap 5, Bootstrap Icons, and custom CSS without frontend frameworks like React or Angular.

---

## 2. Technology Stack

- **Framework:** ASP.NET Core MVC (.NET 10)
- **Language:** C#
- **Database:** MySQL Server 8.0 (`placement_management_db`)
- **ORM / Data Access:** Entity Framework Core 9.0 (Code-First)
- **MySQL Provider:** `Pomelo.EntityFrameworkCore.MySql` (9.0.0)
- **Password Security:** BCrypt password hashing (`BCrypt.Net-Next`)
- **Frontend / Views:** Razor Views (.cshtml), HTML5, CSS3, JavaScript
- **CSS Framework:** Bootstrap 5.3 & Bootstrap Icons 1.11

---

## 3. User Roles & Features

### 👨‍💼 1. Admin / Placement Officer
- **Dashboard:** Real-time metrics on total students, partner companies, active drives, placed students, average and highest CTC salary packages, recent applications, and upcoming recruitment drives.
- **Student CRUD:** Add, browse, search by name/enrollment, filter by department, minimum CGPA, view profile, edit, and delete students.
- **Company CRUD:** Register recruiting companies, browse industry directory, manage HR contacts, edit, and delete companies.
- **Placement Drive CRUD:** Create placement drives with company dropdown (`<select>`), set minimum CGPA, eligible departments, package, drive date, application deadline, and vacancies.
- **Applications Management:** View all student applications, filter by status (`Applied`, `Shortlisted`, `Interview`, `Selected`, `Rejected`), review student academic records and resumes, update candidate status, and add evaluator remarks.
- **Placement Records CRUD:** Automatically or manually create official placement records for selected students, track CTC packages, edit, and maintain college placement statistics.

### 🎓 2. Student
- **Registration & Authentication:** Register with Enrollment Number, Full Name, Email, Phone, Department, Semester, CGPA, Skills, and Password. Login via Email or Enrollment Number.
- **Personalized Dashboard:** Welcome panel, live count of eligible drives, applied drives, shortlisted count, selected count, and congratulations banner if placed.
- **Profile Management:** View and edit personal information, CGPA, technical skills, and upload resume documents (.pdf, .docx).
- **Placement Drive Listing:** Browse drives with real-time eligibility evaluation (CGPA criteria, department criteria, deadline check, duplicate check).
- **1-Click Application:** Apply for eligible drives with server-enforced validation preventing duplicate submissions or late entries.
- **My Applications:** Track application progress across recruitment stages (`Applied` ➔ `Shortlisted` ➔ `Interview` ➔ `Selected` / `Rejected`) with evaluator remarks.
- **My Placement Status:** Access official placement confirmation letter with company name, job role, and verified salary package.

---

## 4. Database Schema & Entity Relationships

The MySQL database is named `placement_management_db`. The relationships configured in `ApplicationDbContext` are:

```
Company (1)  ────────< (Many) PlacementDrive
                                  |
                                  | (1)
                                  v
Student (1)  ────────< (Many) Application
     |                            |
     | (1)                        | (1)
     v                            v
Placement  ──────────< (Many) Placement
```

### Main Entities & Validations:
- **`Student`:** `Id`, `EnrollmentNo` (Unique), `FullName`, `Email` (Unique), `Phone`, `DateOfBirth`, `Gender`, `Department`, `Semester` (1–8), `CGPA` (0–10), `Skills`, `ResumeFileName`, `ProfilePicture`, `Password` (Hashed), `Role`, `CreatedAt`.
- **`Company`:** `Id`, `CompanyName`, `CompanyDescription`, `Industry`, `Location`, `Website`, `HRName`, `HREmail`, `ContactNumber`, `CreatedAt`.
- **`PlacementDrive`:** `Id`, `CompanyId` (FK), `JobTitle`, `JobDescription`, `EligibilityCGPA`, `EligibleDepartment`, `RequiredSkills`, `SalaryPackage` (LPA), `JobLocation`, `DriveDate`, `ApplicationDeadline`, `NumberOfVacancies`, `Status`, `CreatedAt`.
- **`Application`:** `Id`, `StudentId` (FK), `PlacementDriveId` (FK), `ApplicationDate`, `Status` (`Applied`, `Shortlisted`, `Interview`, `Selected`, `Rejected`), `Remarks`. *Unique composite constraint on `(StudentId, PlacementDriveId)` prevents duplicate applications.*
- **`Placement`:** `Id`, `StudentId` (FK), `CompanyId` (FK), `PlacementDriveId` (FK), `PlacementDate`, `Package`, `JobRole`, `Status` (`Placed`, `Pending`, `Cancelled`).
- **`AdminUser`:** `Id`, `FullName`, `Email`, `Password` (Hashed), `Role` (`Admin`), `CreatedAt`.

---

## 5. MVC Project Structure

```text
PlacementManagementSystem/
│
├── Controllers/
│   ├── HomeController.cs           # Landing page, public stats, active drive preview
│   ├── AccountController.cs        # Login, registration, session authentication, logout
│   ├── StudentController.cs        # Admin & Student dashboards, profile edit, student CRUD
│   ├── CompanyController.cs        # Company directory, search, filter, and CRUD
│   ├── PlacementDriveController.cs # Drive listing, eligibility checking, details, CRUD
│   ├── ApplicationController.cs    # Apply, duplicate prevention, status review, MyApplications
│   └── PlacementController.cs      # Official placement records, salary stats, student offer
│
├── Models/
│   ├── Student.cs                  # Student entity with DataAnnotations
│   ├── Company.cs                  # Company entity
│   ├── PlacementDrive.cs           # Placement drive entity
│   ├── Application.cs              # Application entity with foreign keys
│   ├── Placement.cs                # Placement record entity
│   └── AdminUser.cs                # Placement officer credentials
│
├── ViewModels/
│   ├── LoginViewModel.cs           # Username/email, password, return URL
│   ├── RegisterViewModel.cs        # Comprehensive registration fields & compare validators
│   ├── DashboardViewModel.cs       # Admin & Student statistical view models
│   ├── StudentProfileViewModel.cs  # Profile view/update with resume upload
│   └── PlacementCreateViewModel.cs # Placement creation with auto pre-filling
│
├── Data/
│   ├── ApplicationDbContext.cs     # EF Core MySQL context, fluent API configuration
│   └── DbInitializer.cs            # Automatic seed data for Admin, Students, Companies & Drives
│
├── Helpers/
│   ├── PasswordHelper.cs           # BCrypt password hashing & verification
│   ├── SessionExtensions.cs        # Typed session helper methods & role checks
│   └── AuthorizeRoleAttribute.cs   # ActionFilter attribute for role-based route security
│
├── Views/
│   ├── Home/ (Index, About)
│   ├── Account/ (Login, Register, AccessDenied)
│   ├── Student/ (StudentDashboard, AdminDashboard, Profile, Index, Details, Create, Edit, Delete)
│   ├── Company/ (Index, Details, Create, Edit, Delete)
│   ├── PlacementDrive/ (Index, Details, Create, Edit, Delete)
│   ├── Application/ (Index, MyApplications, Details)
│   ├── Placement/ (Index, MyPlacement, Details, Create, Edit, Delete)
│   └── Shared/ (_Layout, _NotificationPartial, _ValidationScriptsPartial, Error)
│
├── wwwroot/
│   ├── css/site.css                # Custom modern design system
│   ├── js/site.js
│   └── uploads/resumes/            # Student resume storage directory
│
├── appsettings.json                # MySQL connection string configuration
├── Program.cs                      # Service registration, EF Core, Session & pipeline setup
├── placement_management_db.sql     # Standalone SQL export for manual MySQL import
├── start_mysql.bat                 # Helper script to launch local MySQL on port 3307
├── run_project.bat                 # Helper script to launch web application
└── PlacementManagementSystem.csproj
```

---

## 6. How to Run the Project

### Prerequisites:
1. [.NET 10 SDK](https://dotnet.microsoft.com/)
2. MySQL Server 8.0 running on localhost

### Step 1: Configure MySQL Connection String
Open `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3307;database=placement_management_db;user=root;password=;"
}
```
*(If your MySQL is running on standard port 3306 with a password, change port to `3306` and set `password=YOUR_PASSWORD;`)*

### Step 2: Restore Dependencies & Build
```bash
cd PlacementManagementSystem
dotnet restore
dotnet build
```

### Step 3: Run Database Migrations (EF Core)
```bash
dotnet dotnet-ef database update
```
*(Or import the pre-generated `placement_management_db.sql` directly into MySQL Workbench / phpMyAdmin)*

### Step 4: Run the Web Application
```bash
dotnet run --urls "http://localhost:5000"
```
Or simply double-click `run_project.bat`!

Open your browser and navigate to:
👉 **`http://localhost:5000`**

---

## 7. Demo Login Credentials

| Role | Username / Email | Password | Details |
|---|---|---|---|
| **Placement Officer (Admin)** | `admin@college.com` | `Admin@123` | Full access to manage students, companies, drives, applications, and placement records |
| **Sample Student 1** | `21BCSE001` or `aarav.sharma@college.com` | `Student@123` | CSE Department, CGPA 8.85 (Eligible for Microsoft, TCS, GS) |
| **Sample Student 2** | `21BCSE045` or `diya.patel@college.com` | `Student@123` | CSE Department, CGPA 9.20 (Placed at Microsoft India, 22.5 LPA) |
| **Sample Student 3** | `21BIT012` or `rohan.verma@college.com` | `Student@123` | IT Department, CGPA 7.65 (Placed at TCS, 7.2 LPA) |
| **Sample Student 4** | `21BCSE088` or `kabir.singh@college.com` | `Student@123` | CSE Department, CGPA 6.80 (Demonstrates CGPA ineligibility on high-cutoff drives) |

---

## 8. Viva Voce & Presentation Q&A (B.Tech CSE)

### Q1: What is the Model-View-Controller (MVC) architecture?
**Answer:** MVC is an architectural software design pattern:
- **Model:** Represents the data structure, business entities, and database logic (`Student.cs`, `Company.cs`, `ApplicationDbContext.cs`).
- **View:** Represents the UI presented to the end user (`Index.cshtml`, `Details.cshtml`), rendering dynamic HTML using Razor syntax.
- **Controller:** Acts as an intermediary handling HTTP requests, coordinating business rules, querying models via EF Core, and returning the appropriate Views (`StudentController.cs`, `ApplicationController.cs`).

### Q2: What is Pomelo.EntityFrameworkCore.MySql?
**Answer:** It is an open-source, high-performance Entity Framework Core database provider for MySQL and MariaDB, built on top of `MySqlConnector`. It allows EF Core to translate LINQ queries into optimized MySQL SQL statements.

### Q3: How is state management handled in this project?
**Answer:** 
1. **Session (`HttpContext.Session`):** Used to store user identity across requests (`UserId`, `UserName`, `UserRole`) after successful credential validation. Session timeout is set to 30 minutes.
2. **TempData (`TempData["Success"]` / `TempData["Error"]`):** Used to persist flash messages across controller redirects (PRG - Post/Redirect/Get pattern).

### Q4: How are duplicate applications prevented?
**Answer:**
1. **Controller logic:** Queries `_context.Applications.AnyAsync(a => a.StudentId == studentId && a.PlacementDriveId == driveId)` before creating a record.
2. **Database layer:** A composite unique index on `(StudentId, PlacementDriveId)` configured in `ApplicationDbContext` guarantees database-level integrity.

### Q5: What is `[ValidateAntiForgeryToken]` and why is it used?
**Answer:** It protects POST forms against Cross-Site Request Forgery (CSRF) attacks. The server generates a cryptographic token embedded in the form and validates that requests originated genuinely from the authenticated client session.

---

### Developed for Academic Submission — B.Tech Computer Science & Engineering.
