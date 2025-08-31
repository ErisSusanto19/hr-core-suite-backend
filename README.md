# HR-Core Suite

**HR-Core Suite** is the backend for a modern Human Resource Information System (HRIS) platform, designed to serve as the central hub for managing HR data and processes in an organization securely and efficiently. The project is built on .NET 8 with Clean Architecture principles to ensure scalability, maintainability, and robustness.

This first phase focuses on core features: Employee Master Data Management, Contract Tracking, and a secure JWT-based Authentication system.

---

## Table of Contents

1.  [About the Project](#about-the-project)
2.  [Core Features](#core-features)
3.  [Architecture & Technology](#architecture--technology)
4.  [Getting Started](#getting-started)
    *   [Prerequisites](#prerequisites)
    *   [Installation & Setup](#installation--setup)
5.  [API Structure & Usage](#api-structure--usage)
    *   [Authentication](#authentication)
    *   [Interactive Documentation (Swagger)](#interactive-documentation-swagger)
6.  [Development Roadmap](#development-roadmap)

---

## About the Project

The main goal of **HR-Core Suite** is to transform HR operations from manual processes into an automated, centralized, and data-driven system, while maintaining data security and integrity.

**For HR & Management Teams:**
*   **Single Source of Truth:** Centralized data for employees, branches, and job positions.
*   **Proactive Monitoring:** A flexible API to track expiring employee contracts.
*   **Operational Efficiency:** Batch processing feature to add or update thousands of employee records at once via Excel file upload.
*   **Powerful Search:** Advanced search, filtering, sorting, and pagination capabilities to manage large volumes of employee data.

**For Development Teams:**
*   **Modern & Scalable:** Built on .NET 8 with Clean Architecture.
*   **Secure:** Implemented JWT Bearer authentication and role-based authorization.
*   **Robust:** Features advanced data validation (FluentValidation), global error handling, and a consistent API response structure.
*   **Well-Documented:** Self-documenting interactive API powered by Swagger/OpenAPI.

---

## Core Features

*   **Entity Management:** Full CRUD APIs for Employees, Branches, and Positions.
*   **Authentication & Authorization:**
    *   Internal user registration and JWT-based login system.
    *   Endpoints secured using `[Authorize]` attribute.
    *   Role system with initial data seeding for `Super_Admin` and `HR_Admin`.
*   **Advanced Employee Features:**
    *   Specific endpoint `GET /api/employee/expiring-contracts` for contract monitoring.
    *   Full-text search, filtering by branch/position, dynamic sorting, and pagination.
*   **Batch Processing:** `POST /api/employee/upload` endpoint to process data from Excel files, complete with "upsert" logic and row-by-row error reporting.
*   **Standardized Response Architecture:** All API responses (both success and failure) are wrapped in a consistent JSON format for easy frontend integration.

---

## Architecture & Technology

This project adopts **Clean Architecture** principles to separate business logic from implementation details.

*   **Domain:** Core business entities and fundamental rules.
*   **Application:** Application logic, DTOs, Interfaces, Validators (FluentValidation), and mapping profiles (AutoMapper).
*   **Infrastructure:** Technical implementations like database access (EF Core), external services, and data seeding.
*   **API (Presentation):** The outermost layer exposing functionality through RESTful API endpoints, complete with middleware and filters.

**Key Technologies:**
*   **Framework:** .NET 8 / ASP.NET Core
*   **Database:** SQL Server
*   **ORM:** Entity Framework Core 8
*   **Validation:** FluentValidation
*   **Object Mapping:** AutoMapper
*   **Security:** JWT Bearer Authentication
*   **Excel Processing:** ClosedXML

---

## Getting Started

### Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer Edition)
*   Git

### Installation & Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/ErisSusanto19/hr-core-suite-backend
    cd [hr-core-suite-backend]
    ```

2.  **Configure the Database:**
    *   Open `appsettings.Development.json` in the `HRCoreSuite.API` project.
    *   Ensure the `ConnectionString` matches your local SQL Server configuration.

3.  **Apply Database Migrations:**
    *   Open a terminal in the backend's root directory.
    *   Run the following command. This will create the database, apply all schemas, and run the data seeder (creating the `admin` user and roles).
    ```bash
    dotnet ef database update --startup-project src/HRCoreSuite.API
    ```

4.  **Run the Application:**
    *   Run the following command from the backend's root directory:
    ```bash
    dotnet run --project src/HRCoreSuite.API --launch-profile https
    ```
    *   The API application is now running.

---

## API Structure & Usage

### Authentication

1.  **Initial Login:** Use the `POST /api/auth/login` endpoint with the default seeded credentials to get a JWT:
    *   **Username:** `admin`
    *   **Password:** `admin123`
2.  **Using the Token:** Copy the received token. In the Swagger UI, click the `Authorize` button in the top right, type `Bearer ` (with a space), then paste your token.

### Interactive Documentation (Swagger)

Once the application is running, navigate to `https://localhost:[PORT]/swagger` in your browser to access the complete and interactive API documentation. You can test all endpoints from there.

---

## Development Roadmap

This project is developed in phases.

*   [x] **Phase 1: Backend Foundation (MVP)**
    *   [x] Clean Architecture setup.
    *   [x] CRUD APIs for core entities.
    *   [x] Security System (JWT Authentication & Authorization).
    *   [x] Advanced Features: Search, Pagination, Filtering.
    *   [x] Excel Upload Feature.
    *   [x] Robust Error Handling & Validation.

*   [ ] **Phase 2: Frontend & Additional Features**
    *   [ ] Frontend Development (ASP.NET Core MVC / Razor Pages).
    *   [ ] Employee Self-Service (ESS) portal.
    *   [ ] Online leave request and approval module.

* [ ] **Phase 3: Performance & Payroll Management**

  * [ ] Performance appraisal module.
  * [ ] Automated payroll calculation module.

* [ ] **Phase 4: Analytics & Strategic HR**

  * [ ] HR analytics dashboard.
  * [ ] Custom reporting.

---

## Contributing

This project is still in the early development stage. Contribution guidelines will be added in the future.
