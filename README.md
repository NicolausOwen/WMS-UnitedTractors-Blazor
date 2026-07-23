# Inventory System

<p align="center">
  <img src="WMS-UnitedTracors-Blazor/wwwroot/img/incorp.png" alt="Inventory System Logo" width="300" />
</p>

<p align="center">
  <strong>Enterprise Inventory Management, Asset Borrowing, and Merchandise Distribution System</strong>
</p>

---

## Executive Overview

**Inventory System** is an enterprise-grade warehouse and asset management solution built on the **.NET 9.0 Blazor InteractiveServer** framework. The platform provides end-to-end operational visibility, structured governance, and strict accountability for asset borrowing (*Borrow Catalog*) and promotional item distribution (*Giveaway Catalog*).

Engineered with a multi-tiered approval pipeline, real-time lifecycle tracking, automated PDF document generation, and granular Role-Based Access Control (RBAC), the system ensures seamless operational coordination across requester, inventory staff, administrative, and managerial levels.

---

## Core System Architecture and Capabilities

### 1. Request Catalogs
* **Borrow Catalog**: Facilitates asset borrowing with real-time stock validation, multi-variant options (color and size), expected return date scheduling, and credit point validation.
* **Giveaway Catalog**: Manages promotional merchandise distribution using a user credit quota allocation model.

### 2. Multi-Stage Approval Pipeline
The platform enforces a structured three-tier approval workflow tailored to enterprise governance standards:
* **Stage 1 (Inventory Staff)**: Initial physical inventory check and stock allocation screening.
* **Stage 2 (Admin)**: Administrative verification and operational compliance approval.
* **Stage 3 (Manager)**: Final authorization for high-value or restricted asset requests.
* **Reviewer Transparency**: Comprehensive audit trails recording individual stage approvers, rejection records, and approval timestamps across all views.
* **Applicant Context Retention**: Requisitioner notes (*Catatan Pemohon*) are preserved throughout the pipeline and rendered across all reviewer dashboards.

### 3. Dynamic Revision and Locking Mechanics
* **Interactive Re-submission**: If an approver requests revision (`REVISION_BY_*`), the item transitions to a revisable state.
* **Input Immutability**: Event parameters are locked to read-only during revision to preserve audit integrity.
* **State Locking**: Upon re-submission, the transaction automatically routes back to the requesting approval stage (`PENDING_*`). The revision interface is immediately locked to prevent unauthorized modifications while under review.
* **Conditional Re-engagement**: Controls unlock dynamically only if an approver requests further adjustments.

### 4. Handover and Return Management
* **Proof of Handover (BAST)**: Verification workflow for physical asset handovers with mandatory proof image uploads.
* **User Documentation**: Digital documentation capture for giveaway merchandise distribution.
* **Return Processing**: Physical return confirmation with automated stock balance restoration.

### 5. Automated PDF Document Generation
Powered by QuestPDF, the platform generates audit-ready PDF documents:
* Borrowing Receipts and Return Confirmation Vouchers
* Handover Agreements (*Berita Acara Serah Terima - BAST*)
* User Documentation Summaries
* Periodic Inventory Transaction Audit Reports

### 6. Read-Only Transaction Audit History
The History module provides a strictly read-only repository of finalized transactions (`APPROVED`, `COMPLETED`, `REJECTED`), ensuring record immutability and compliance reporting.

### 7. Role-Based Access Control (RBAC)
Granular permissions mapped across administrative tiers:
* **Super Admin**: System configuration, user management, and role policy assignment.
* **Inventory Staff**: Category-level inventory control, stock adjustments, and Stage 1 approvals.
* **Admin**: Administrative approvals, BAST verification, and operational reporting.
* **Manager**: Stage 3 managerial sign-off and executive oversight.
* **User**: Requisition submission, status tracking, and profile management.

---

## Technical Stack

* **Frontend Framework**: .NET 9.0 Blazor InteractiveServer Mode
* **Backend Runtime**: ASP.NET Core (.NET 9.0)
* **Database & ORM**: MySQL 8.0 with Entity Framework Core 9.0 (`Pomelo.EntityFrameworkCore.MySql`)
* **Styling & Design System**: Tailwind CSS, Custom Utility CSS, Syne & DM Sans Typography
* **PDF Rendering**: QuestPDF Engine
* **Authentication**: ASP.NET Core Cookie Authentication with Custom Claims Architecture

---

## Installation and Deployment Guide

### Prerequisites
* .NET 9.0 SDK or higher
* MySQL Server 8.0 or higher
* Git Version Control

### 1. Repository Setup
```bash
git clone <repository-url>
cd WMS-UnitedTracors-Blazor/WMS-UnitedTracors-Blazor
```

### 2. Database Configuration
Configure the target MySQL connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ut_wms_db;User=root;Password=your_password;"
  }
}
```

### 3. Database Initialization and Seeding
To execute database migrations and initialize baseline seed data:
```bash
dotnet run --reset-db
```
*Note: The `--reset-db` execution argument drops existing schema instances, applies fresh migrations, seeds baseline master data from `Data/SeedData/*.json`, and automatically flags image-less records to `is_hidden = 1`.*

### 4. Application Execution
To launch the application in standard development mode:
```bash
dotnet run
```
Access the application web endpoint at the configured local address (e.g., `https://localhost:7088`).

---

## Directory Structure

* **`Components/Pages/`**: Blazor page components (Catalogs, Tracking, Approval Dashboard, History).
* **`Components/Layout/`**: Navigation layouts, top bar, and drawer navigation components.
* **`Services/`**: Business logic, approval pipeline handlers, PDF generation, and authentication services.
* **`Models/`**: Entity Framework database entities and schema definitions.
* **`Data/SeedData/`**: Master seed data JSON files for automated database provisioning.
* **`wwwroot/img/`**: System graphic assets including `incorp.png`.

---

## License
Copyright © 2026 Inventory System. All rights reserved.
