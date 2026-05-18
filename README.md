# Global Logistics Management System (GLMS) 🎀

An enterprise-grade ASP.NET Core MVC international shipping and SLA management platform designed to track global freight alignments, match organizational contracts, and dynamically calculate transactional conversion costs. 


---

## Project Walkthrough & Presentation
Click the link below to watch a full execution demonstration detailing the system workflows, asynchronous currency calculation infrastructure, and passing testing matrices:

**[Watch the GLMS Part 2 Video Presentation](ADD YOUTUBE LINK HERE)**

---

## Features & Functional Milestones

### 1. Asynchronous Live Currency Auto-Calculation
* **Real-Time Client Updates:** Utilizes native JavaScript `fetch` event listeners to monitor user input values in the currency panel while typing.
* **Backend HttpClient Integration:** The system routes requests through an asynchronous backend wrapper, parsing real-time exchange rates from an external JSON service without requiring full page reloads or layout flashes.

### 2. Comprehensive Navigation & Unified Layout
* **Seamless Redirection:** Completely updated master navbar allowing free traversal across the **Clients Dashboard**, **Manage Contracts**, and **Service Requests** registries.
* **Magical Visuals:** Seamless integration of Bootstrap Icons, centered forms (`form-card`), and beautiful notification sheets.

### 3. Automated Error & Validation Interception
* **SweetAlert2 Feedback:** Fully wired up to catch background model state field anomalies and custom restrictive `TempData` payloads (such as attempting to link orders to an *Expired* or *On Hold* status contract).

---

## Architecture & Tech Stack
* **Framework:** ASP.NET Core 9.0 MVC
* **Database Layer:** Entity Framework Core Code-First with SQL Server
* **Unit Testing Engine:** xUnit Core Test runner
* **Front-End Utility:** Bootstrap 5, SweetAlert2, and Bootstrap Icons

---

## Robust Unit Testing Matrix
The solution features a dedicated testing project (`Global_Logistics_Management_System.Tests`) targeting **.NET 9.0** to validate fundamental business rules. 

### Core Test Suites
1. **Currency Math Verification:** Assures that mathematical multiplications converting USD fractions to local ZAR remain perfectly accurate down to decimal increments under changing mock exchange variables.
2. **File Upload Security Validation:** Assures that path extension interceptors strictly block dangerous or unauthorized extension types (e.g., `.exe`, `.msi`, `.zip`) while permitting clean `.pdf` SLA uploads.
3. **Complex Task Enforcement:** Assures that the underlying domain entities block incoming job allocations if the parent contract status changes to an invalid operational state.


---

## Getting Started & Local Installation

### Prerequisites
* Visual Studio 2022 (with **.NET 9.0 SDK** workload installed)
* SQL Server Express / LocalDB

### 1. Apply Database Migration Scripts
To map the relational data schema context cleanly into your local SQL Server instance, open the **Package Manager Console** and run:
```powershell
Update-Database

### 2. Run the Application
* Open the solution file Global_Logistics_Management_System.sln in Visual Studio.

* Ensure the main web project is set as the startup project.

* Press F5 to build and launch the server pipeline natively.

### 3. Run Automated Unit Tests
To verify code logic integrity directly on your machine:

* Go to Test ➡️ Run All Tests (or use shortcut Ctrl + R, A).

## Developer Credentials

**Developer Name:** Koketso Masasanya

**Student Identification Number:** ST10225793


