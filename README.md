<p align="center">
  <img src="WMS-UnitedTracors-Blazor/wwwroot/img/logo.svg" alt="WMS United Tractors Logo" width="150" />
</p>

<h1 align="center">WMS United Tractors</h1>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/Blazor-Web_App-512BD4?logo=blazor&logoColor=white" alt="Blazor" />
  <img src="https://img.shields.io/badge/Database-MySQL-4479A1?logo=mysql&logoColor=white" alt="MySQL" />
  <img src="https://img.shields.io/badge/ORM-EF_Core-512BD4?logo=nuget&logoColor=white" alt="Entity Framework Core" />
  <img src="https://img.shields.io/badge/Deployed_on-Azure-0089D6?logo=microsoft-azure&logoColor=white" alt="Azure" />
</p>

Welcome to the WMS United Tractors repository. This project is a comprehensive Warehouse Management System (WMS) built using a modern .NET technology stack to provide robust inventory control and asset tracking.

**Live Application:** [WMS United Tractors Azure Environment](https://wmsut-cne5e8f0hyejh3e7.southeastasia-01.azurewebsites.net)

---

## Framework & Technology Stack

This system is developed utilizing the following core technologies:

* **Frontend / UI:** [.NET 10 Blazor Web App](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) (Interactive Server Mode) with Razor Components.
* **Backend:** ASP.NET Core (.NET 10).
* **Database:** MySQL, managed via Entity Framework Core (`Pomelo.EntityFrameworkCore.MySql`).
* **Authentication & Integrations:** Microsoft Entra ID (Azure AD) and Azure Communication Services (Email).
* **Styling:** Custom CSS and modern component libraries.

---

## System Features

The architecture is tailored to support end-to-end inventory management workflows:

1. **Dashboard & Analytics:** Real-time visibility into inventory stock, active transactions, and operational metrics.
2. **Master Data Management:** Administrative control over Categories, Locations, Units, Products, and Product Variants.
3. **Transaction Workflows (Borrow & Giveaway):**
   * Multi-tiered approval logic customized by user role: Requester -> Approver (Head) -> Manager -> Admin (Staff Inventory).
   * Capability to upload documentation and handover evidence directly into the transaction lifecycle.
4. **Role-Based Access Control (RBAC):**
   * Granular permission structures restricting data access and authorized actions based on user roles.
   * Auto-hashed BCrypt passwords for locally managed user accounts.
5. **Automated Database Seeding:** A robust utility to reset, migrate, and seed the database using localized JSON datasets (`Data/SeedData`). The routine is engineered to preserve initial baseline stock metrics while actively purging previous transactional deductions to ensure a clean development environment.

---

## Setup & Installation

Follow these instructions to deploy and execute the application in a local development environment.

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* A MySQL database instance (local or remote)
* Git version control

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd WMS-UnitedTracors-Blazor
cd WMS-UnitedTracors-Blazor
```

### 2. Configure Environment Settings

Open the `appsettings.json` file located in the project root directory. Modify the `DefaultConnection` string with your applicable MySQL credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=ut_wms_db;User=root;Password=yourpassword;"
}
```
*(Note: Ensure that your Azure AD and SMTP configuration settings in this file are appropriately populated if testing SSO or email dispatch mechanisms).*

### 3. Restore Dependencies

Restore all necessary NuGet packages by running:

```bash
dotnet restore
```

### 4. Database Migration & Clean Seeding

This solution includes a custom utility command to automatically execute Entity Framework migrations and seed the master data natively.

To perform a clean reset and seed the database, run:

```bash
dotnet run --reset-db
```

> **Important:** The `--reset-db` argument will drop and recreate existing tables, apply new migrations, and populate master data directly from the `Data/SeedData/*.json` files. Transaction histories are purged, and variant stock levels are reset to their baseline condition.

### 5. Start the Application

For a standard execution without invoking the database reset sequence:

```bash
dotnet run
```

The application will initialize, and it will be accessible via the address and port specified in the terminal output (e.g., `http://localhost:5285`).

---

## Project Structure Overview

* **`/Models`**: Contains the Entity Framework Core entities and database schema mappings.
* **`/Components`**: Houses reusable Razor components and UI Pages for the Blazor frontend.
* **`/Data/SeedData`**: Contains the JSON source files leveraged by `DbSeeder.cs` for automated database seeding.
* **`/Services`**: Implements core business logic, authentication handling, and approval workflow processing.
* **`Program.cs`**: The main entry point, managing Dependency Injection registration and the execution pipeline (including the custom `--reset-db` argument processing).

---

*For technical inquiries or system support, please escalate to the development team or submit an issue within the repository.*