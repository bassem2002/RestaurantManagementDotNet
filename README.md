# 🍽️ RestaurantManagementDotNet

<p align="center">
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core" />
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/JWT%20%7C%20RBAC-Security-0F766E?style=for-the-badge" alt="JWT RBAC" />
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-14B8A6?style=for-the-badge" alt="MIT License" /></a>
</p>

<p align="center"><a href="#features">Features</a> · <a href="#architecture">Architecture</a> · <a href="#️-getting-started">Getting started</a></p>

Restaurant management application built with **ASP.NET Core**, **Entity Framework Core**, and **SQL Server**, featuring JWT authentication, ASP.NET Core Identity, product catalog management, user roles, and repository-based data access.

---

## ✨ Overview

**RestaurantManagementDotNet** is a restaurant management solution designed around an ASP.NET Core REST API.

The backend provides authentication, user-role management, product and category management, shopping-cart initialization, and database persistence through Entity Framework Core.

The project follows a structured architecture based on **Controllers**, **Repository Interfaces**, **Repository Implementations**, and **Entity Framework Core**.

---

## 🚀 Key Features

### 🔐 Authentication & User Management

The application uses **ASP.NET Core Identity** for user and password management together with **JWT Bearer Authentication** for API access.

Implemented features include:

- User registration
- User login
- Password management through ASP.NET Core Identity
- Automatic assignment of the `Client` role after registration
- `Admin` and `Client` application roles
- JWT generation after successful authentication
- Identity and role claims included in JWT tokens
- Two-hour JWT expiration
- Automatic shopping-cart creation for newly registered clients

---

## 🛍️ Product Catalog Management

The REST API provides product catalog management features.

Users can:

- Retrieve all products
- Retrieve a product by ID
- Associate products with categories
- Create products
- Update product information
- Update product pricing
- Delete products
- Upload product images
- Serve product images through ASP.NET Core static files

Product data is persisted using the repository layer and Entity Framework Core.

---

## 🏗️ Architecture

The backend follows a layered structure using dependency injection and repository abstractions.

```text
HTTP Request
     │
     ▼
Controller
     │
     ▼
Repository Interface
     │
     ▼
Repository Implementation
     │
     ▼
Entity Framework Core
     │
     ▼
AppDbContext
     │
     ▼
SQL Server
```

This structure separates HTTP/API concerns from database-access logic and makes the application easier to maintain and extend.

### End-to-End Application Flow

```text
┌──────────────────────────────────────────────┐
│ Blazor WebAssembly Frontend                  │
│ Admin UI · Client UI · JWT state · Services  │
└──────────────────────┬───────────────────────┘
                       │ HTTPS / JSON REST API
                       │ Authorization: Bearer <JWT>
                       ▼
┌──────────────────────────────────────────────┐
│ ASP.NET Core Backend                         │
│ Controllers · DTOs · Identity · Authorization│
└──────────────────────┬───────────────────────┘
                       │ Repository interfaces
                       ▼
┌──────────────────────────────────────────────┐
│ Repository Implementations                   │
│ Business-oriented data access                │
└──────────────────────┬───────────────────────┘
                       │ Entity Framework Core
                       ▼
┌──────────────────────────────────────────────┐
│ SQL Server / LocalDB                         │
│ Users · Roles · Items · Carts · Orders       │
└──────────────────────────────────────────────┘
```

The Blazor client sends JSON requests to the ASP.NET Core API. Authentication tokens are stored on the client and attached to protected requests. Controllers validate and map incoming data through DTOs, repository implementations use `AppDbContext`, and Entity Framework Core persists the domain entities in SQL Server.

### Main Backend Layers

```text
Backend/
│
├── Controllers/
│   └── REST API endpoints
│
├── DTOs/
│   └── Request and response data transfer objects
│
├── Data/
│   └── Entity Framework Core database context
│
├── Repository/
│   ├── Interfaces/
│   └── Implementations/
│
├── Migrations/
│   └── Entity Framework Core migrations
│
├── wwwroot/
│   └── Static resources and uploaded product images
│
├── Program.cs
│   └── Application configuration and dependency injection
│
└── appsettings.json
    └── Application configuration
```

## 📂 Complete Repository Structure

```text
RestaurantManagementDotNet/
├── Backend/                         # ASP.NET Core REST API
│   ├── Controllers/                 # Account, catalog, cart, order and payment APIs
│   ├── Data/
│   │   └── AppDbContext.cs          # EF Core and Identity database context
│   ├── DTOs/                        # API request and response contracts
│   ├── Migrations/                  # SQL Server schema history
│   ├── Repository/
│   │   ├── Interfaces/              # Data-access abstractions
│   │   └── Implementations/         # EF Core repository implementations
│   ├── Properties/                  # Development launch profiles
│   ├── wwwroot/uploads/items/       # Uploaded product images
│   ├── Program.cs                   # Services, security and middleware
│   ├── appsettings.json             # Local application configuration
│   └── Backend.csproj
├── Front/                           # Blazor WebAssembly client
│   ├── Layout/                      # Admin, client and navigation layouts
│   ├── Models/                      # Frontend request and view models
│   ├── Pages/
│   │   ├── Admin/                   # Dashboard, clients, categories, items and orders
│   │   ├── Client/                  # Catalog, cart, checkout, orders and profile
│   │   ├── Login.razor
│   │   └── Register.razor
│   ├── Services/                    # API, token, authentication and UI services
│   ├── wwwroot/                     # CSS, icons, PWA files and static assets
│   ├── App.razor                    # Routing and authorization shell
│   ├── Program.cs                   # Client dependency injection and API base URL
│   └── Front.csproj
├── Models/                          # Domain entities shared by API and client
│   ├── ApplicationUser.cs
│   ├── Cart.cs
│   ├── CartItem.cs
│   ├── Category.cs
│   ├── Item.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Payment.cs
│   └── Models.csproj
├── ProjetResto.sln                  # .NET solution containing all three projects
├── MIGRATION_INSTRUCTIONS.md        # Database migration notes
├── ROADMAP_DEVELOPPEMENT.md         # Development roadmap
├── LICENSE
└── README.md
```

---

## 🔑 Authentication Flow

### Registration

```text
Client Registration
        │
        ▼
ASP.NET Core Identity
        │
        ▼
User Creation
        │
        ▼
Client Role Assignment
        │
        ▼
Shopping Cart Creation
```

### Login

```text
Login Request
        │
        ▼
Credential Validation
        │
        ▼
Load User Roles
        │
        ▼
Generate JWT
        │
        ▼
Add Identity & Role Claims
        │
        ▼
Return JWT to Client
```

JWT tokens contain:

- User identifier
- Username
- User roles
- Token expiration

---

## 🛠️ Technology Stack

### Backend

| Technology | Purpose |
|---|---|
| **C#** | Main programming language |
| **ASP.NET Core** | REST API framework |
| **ASP.NET Core Identity** | User and password management |
| **Entity Framework Core** | Object-relational mapping |
| **SQL Server / LocalDB** | Relational database |
| **JWT** | API authentication |
| **Swagger / OpenAPI** | API documentation and testing |
| **Repository Pattern** | Data-access abstraction |
| **Dependency Injection** | Service and repository management |

### API & Infrastructure

- REST API
- JWT Bearer Authentication
- Admin / Client role model
- ASP.NET Core authorization policies
- HTTPS redirection
- CORS configuration
- Swagger UI
- Entity Framework migrations
- Static-file serving

---

## 👥 User Roles

The application defines two main roles:

| Role | Description |
|---|---|
| **Admin** | Administrative role for management operations |
| **Client** | Default role assigned to registered customers |

Newly registered users automatically receive the `Client` role.

The backend also defines an `AdminOnly` authorization policy for administrative operations.

---

## 🗄️ Database

The application uses **SQL Server** with **Entity Framework Core**.

Database access is configured through:

```text
ASP.NET Core
      ↓
Repository Layer
      ↓
Entity Framework Core
      ↓
AppDbContext
      ↓
SQL Server
```

Entity Framework migrations are included in the project to manage database schema evolution.

---

## 📚 API Documentation

The backend integrates **Swagger / OpenAPI**.

When the application runs in development mode, Swagger UI can be used to explore and test the REST API.

```text
https://localhost:<backend-port>/swagger
```

JWT Bearer authentication is also configured in Swagger.

---

## 🔒 Configuration & Security

Sensitive values such as the JWT signing key should not be committed to the repository.

For local development, secrets should be stored using **.NET User Secrets** or environment variables.

Example:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" "YOUR_LOCAL_SECRET"
```

The public repository should never contain real passwords, production credentials, private certificates, or authentication secrets.

---

## ▶️ Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/RestaurantManagementDotNet.git
cd RestaurantManagementDotNet
```

### 2. Restore .NET dependencies

```bash
dotnet restore
```

### 3. Configure the JWT secret

From the backend project:

```bash
cd Backend

dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" "YOUR_STRONG_SECRET_KEY"
```

### 4. Apply database migrations

```bash
dotnet ef database update
```

### 5. Start the backend

```bash
dotnet run
```

The Swagger interface will be available from the development URL displayed in the terminal.

---

## 🖥️ Frontend Status

The repository includes an implemented **Blazor WebAssembly frontend targeting .NET 9**. It is not an empty scaffold or a future placeholder. The solution currently builds successfully with **zero warnings and zero errors**.

Implemented frontend areas include:

- Login, registration, logout and JWT-based authentication state
- Separate Admin and Client layouts
- Admin dashboard and order monitoring
- Category and product creation, editing and listing
- Client and shopping-cart inspection for administrators
- Client catalog browsing and product display
- Shopping cart and checkout workflow
- Order confirmation, history and tracking
- Client profile management
- Local token storage, route authorization, notifications and toast messages
- Progressive Web App assets and service workers

The frontend communicates with the backend through typed services and `HttpClient`. Its development API base address is currently configured in `Front/Program.cs` as:

```text
https://localhost:7281/
```

To run it, keep the backend active and start the client in a second terminal:

```bash
cd Front
dotnet run
```

The frontend is functional at source and build level, but the public README does not yet include real interface screenshots or a hosted demonstration. The API address is also hard-coded for local development and should be moved to environment-specific configuration before deployment.

---

## 📌 Current Modules

The backend currently contains controllers for:

```text
Account
Cart
Cart Item
Category
Client
Item
Order
Order Item
Payment
```

These modules form the foundation of the restaurant management workflow.

---

## 🧩 Design Choices

This project demonstrates practical use of:

- RESTful API development with ASP.NET Core
- ASP.NET Core Identity
- JWT-based authentication
- Role claims
- Dependency Injection
- Repository abstractions
- Entity Framework Core
- Relational database persistence
- DTO-based API communication
- Product image handling
- Swagger API documentation

---

## 📷 Application Preview

Screenshots of the application interface will be added here.

```text
Application screenshots
Admin interface
Client interface
Product catalog
Shopping cart
Order workflow
```

---

## 🎥 Demo

A demonstration video of the complete application workflow can be added here.

---

## 📈 Future Improvements

Potential improvements include:

- Stronger authorization coverage across administrative endpoints
- More robust image-upload validation
- Additional automated tests
- Centralized exception handling
- Improved configuration management
- Extended API validation

---

## 👨‍💻 Author

**Bassem Wali**

Software Engineering Student  
Full-Stack Development • Backend Development • Applied AI
