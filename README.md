# HR-Core Suite

**HR-Core Suite** is a modern Human Resource Information System (HRIS) platform designed to serve as the central hub for managing HR data and processes in an organization. The project is built with a solid foundation for scalability, starting with the essential modules: Master Data Management and Employee Contracts.

---

## Table of Contents

1. [About the Project](#about-the-project)
2. [Architecture](#architecture)
3. [Built With](#built-with)
4. [Getting Started](#getting-started)
   * [Prerequisites](#prerequisites)
   * [Installation](#installation)
5. [Development Roadmap](#development-roadmap)
6. [Contributing](#contributing)

---

## About the Project

The main goal of **HR-Core Suite** is to transform HR operations from manual, error-prone processes into an automated, centralized, and data-driven system.

**For HR & Management Teams:**

* **Data Centralization:** A single source of truth for all employee data, from personal information and job history to contract details.
* **Proactive Monitoring:** Receive automatic notifications and reports for expiring employee contracts, reducing the risk of oversight.
* **Operational Efficiency:** Speed up onboarding and bulk updates through Excel import features.
* **Decision Support:** Provide accurate and structured data as a foundation for future HR analytics.

**For Development Teams:**

* **Modern & Scalable:** Built on .NET 8 with Clean Architecture, ensuring the codebase is maintainable, testable, and extendable.
* **API-First:** Designed with an API-first approach, enabling integration with various frontends (Web, Mobile) or third-party systems.
* **Clear Documentation:** Structured documentation to make onboarding new developers easier.

---

## Architecture

This project adopts the **Clean Architecture** principle to separate business logic from implementation details. The application is structured into several main layers:

* **Domain:** The core of the application, containing business entities (e.g., `Employee`, `Contract`) and fundamental business rules.
* **Application:** Contains application logic and workflows (use cases), such as data validation and employee data storage.
* **Infrastructure:** Provides technical implementations for external services such as database access (via Entity Framework Core), file systems, or notification services.
* **API (Presentation):** The outermost layer exposing application functionality through RESTful API endpoints.

This separation ensures flexibility and adaptability to future technology changes.

---

## Built With

* **Backend:** [.NET 8](https://dotnet.microsoft.com/en-us/)
* **API Framework:** [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
* **Database:** [SQL Server](https://www.microsoft.com/en-us/sql-server)
* **ORM:** [Entity Framework Core 8](https://docs.microsoft.com/en-us/ef/core/)

---

## Getting Started

Follow the steps below to run this project locally.

### Prerequisites

Ensure your machine has the following installed:

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer Edition is sufficient)
* Git

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/ErisSusanto19/hr-core-suite-backend
   cd hr-core-suite-backend
   ```

2. **Configure the Database:**

   * Open `appsettings.Development.json` in the `HRCoreSuite.API` project.
   * Update the `ConnectionString` to match your local SQL Server configuration.

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HRCoreSuiteDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
   }
   ```

3. **Apply Database Migrations:**

   * Open a terminal or command prompt in the `HRCoreSuite.API` directory.
   * Run the following command to create the database and tables:

   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**

   * Still in the `HRCoreSuite.API` directory, run:

   ```bash
   dotnet run
   ```

   * The API is now running and ready to accept requests at `https://localhost:7001` or `http://localhost:5001`.

---

## Development Roadmap

This project will be developed in phases:

* [x] **Phase 1: Employee Data & Contract Management (MVP)**

  * [ ] Centralized database for employee master data.
  * [ ] Bulk upload & update via Excel.
  * [ ] API for CRUD operations on employees, branches, and job positions.
  * [ ] API for filtering employees based on contract status.

* [ ] **Phase 2: Self-Service & Attendance Management**

  * [ ] Employee Self-Service (ESS) login portal.
  * [ ] Online leave request and approval module.
  * [ ] Attendance management module.

* [ ] **Phase 3: Performance & Payroll Management**

  * [ ] Performance appraisal module.
  * [ ] Automated payroll calculation module.

* [ ] **Phase 4: Analytics & Strategic HR**

  * [ ] HR analytics dashboard.
  * [ ] Custom reporting.

---

## Contributing

This project is still in the early development stage. Contribution guidelines will be added in the future.
