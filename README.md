# SPLM - Software Project Lifecycle Management System

SPLM is a full-stack Software Project Lifecycle Management system designed to help teams manage software projects, track features, plan releases, and monitor project lifecycle stages in an organized and scalable way.

The system includes a backend API built with .NET and a frontend application built with Angular. It follows modern software engineering practices such as Clean Architecture, CQRS, JWT authentication, and role-based access control.

---

## 🚀 Tech Stack

### Backend
- **Framework:** .NET / C#
- **Architecture:** Clean Architecture
- **Pattern:** CQRS with MediatR
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** JWT Authentication
- **Authorization:** Role-Based Access Control (RBAC)

### Frontend
- **Framework:** Angular
- **UI Library:** PrimeNG
- **Styling:** SCSS
- **API Communication:** RESTful APIs

---

## 🏗 Architecture Overview

The backend follows Clean Architecture principles to separate business logic, application rules, infrastructure concerns, and API endpoints.

The main layers are:

- **Domain Layer:** Contains core business entities and domain rules.
- **Application Layer:** Handles use cases, commands, queries, validations, and business logic.
- **Infrastructure Layer:** Manages database access, persistence, and external services.
- **API Layer:** Exposes RESTful endpoints for the frontend application.

This structure helps improve maintainability, scalability, and testability.

---

## ✨ Key Features

- Project lifecycle management
- Feature tracking
- Release planning
- User authentication
- Role-based authorization
- Dashboard-ready backend structure
- Clean and scalable API design
- Angular-based user interface
- SQL Server database integration
- Entity Framework Core migrations

---

## 📊 Main Modules

### Project Lifecycle Management
Manage software projects and track their progress through different lifecycle stages.

### Feature Management
Create, update, and track features from planning to delivery.

### Release Planning
Organize project releases, manage release stages, and support delivery planning.

### User & Role Management
Handle user authentication and restrict access based on roles and permissions.

---

## 📁 Repository Structure

```bash
SPLM/
│
├── backend/
│   └── Backend API source code
│
├── frontend/
│   └── Angular frontend application
│
├── CODEOWNERS
├── LICENCE
└── README.md
