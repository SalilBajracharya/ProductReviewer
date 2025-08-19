# Product Reviewer API

## Overview

A  modular .NET 9 Web API built using Clean Architecture and CQRS principles. This application allows users to register, log in, view products, add reviews, and generate reports all with clean separation of concerns and centralized error logging.

## Features

- JWT Authentication & Role-based Access Control
- Product listing, filtering, and categorization
- Review submission and rating logic on to [Reviewer, Admin] roles
- CSV report generation for products
- Pagination support for [list] endpoints
- Centralized error/exception logging via Serilog
- Global exception handling middleware
- xUnit and Moq-based unit testing
- FluentResults for all types of response handling

## Tecch Stack

- .NET 9
- Entity Framework Core
- MediatR (CQRS)
- Serilog (file-based logging)
- FluentResults
- xUnit + Moq

## Project Structure

ProductReviewer.Api              # Entry point with controllers and middleware
ProductReviewer.Application      # CQRS queries, commands, DTOs
ProductReviewer.Infrastructure   # EF Core, database context, repositories, services
ProductReviewer.Domain           # Entities and enums (Product, Review, etc.)
ProductReviewer.Test.*           # Unit tests for handlers and services


## Running Locally

1. Clone this repository
2. Set your connection string in `appsettings.json`
3. Run the project with startup project ProductReviewer.API

Swagger UI will be available at: `http://localhost:<port>/swagger`

## Workflow

1. Database Initialization:
    * Database and tables are auto-created on first run using migrations.
    * DbInitializer.cs seeds sample users, products, and a default admin.

2. Default Admin:
    * A default Admin is seeded:
        Username: Super
        Password: MyPassword1!

3. Review Permissions:
    * Only users with Admin or Reviewer roles can add product reviews.
    * Default registered users receive the User role, which cannot post reviews.

4. Role Management:
    * Admins can promote users to Reviewer via the protected endpoint:
    POST /Auth/assign-role (Admin-only access)

5. Flow to Add Reviews:
    * Admin assigns the Reviewer role => user logs in => user can now post reviews.

6. Generate CSV report:
    * All authenticated users can generate product review CSV reports.
    * You can restrict this further using Authorization Policies if needed.

## 🧾 Notes
- Logging is written to `/Logs/log-<date>.txt` (rotated daily, retained for 7 days)
- All unhandled exceptions are caught and logged via middleware
- Clean separation of concerns between Application, Infrastructure, Domain and API layers