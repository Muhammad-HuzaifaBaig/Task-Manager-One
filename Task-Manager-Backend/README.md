# Task Manager API

A secure, scalable RESTful API for task management built with **.NET 8**, featuring JWT authentication, BCrypt password hashing, and a clean 3-tier architecture.

---

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Frontend Application](#-frontend-application)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Security](#-security)
- [Configuration](#-configuration)
- [Development Guidelines](#-development-guidelines)

---

## ✨ Features

- 🔐 **JWT Authentication** - Secure token-based authentication
- 🔒 **BCrypt Password Hashing** - Industry-standard password encryption
- 👤 **User Management** - Signup, login, and role-based access control
- 📊 **Task Management** - Create, read, update, and delete tasks
- 🏗️ **3-Tier Architecture** - Separation of concerns with API, Business, and Data layers
- 🔑 **User Secrets** - Secure credential management for development
- 📝 **Standardized Response Format** - Consistent API responses with HTTP status codes
- 🗄️ **SQL Server Database** - Robust data persistence with Entity Framework Core

---

## 🏛️ Architecture

This project follows a **3-Tier Layered Architecture** for maintainability and scalability:

```
┌─────────────────────────────────────────────┐
│          TaskManager.Api (Presentation)      │
│  Controllers, Middleware, Configuration     │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│       TaskManager.Business (Business Logic)  │
│  Services, Helpers, Business Rules, DTOs    │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│      TaskManager.Data (Data Access Layer)    │
│  Repositories, DbContext, EF Core Migrations│
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│        TaskManager.Model (Entities & DTOs)   │
│  Domain Models, Request/Response DTOs       │
└─────────────────────────────────────────────┘
```

**Benefits:**
- **Separation of Concerns** - Each layer has a single responsibility
- **Testability** - Layers can be tested independently
- **Maintainability** - Changes in one layer don't affect others
- **Scalability** - Easy to extend and add new features

---

## 🛠️ Tech Stack

### **Backend**
- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Relational database

### **Security**
- **JWT (JSON Web Tokens)** - Stateless authentication
- **BCrypt.Net** - Password hashing
- **User Secrets** - Secure local credential storage
- **ASP.NET Core Identity** principles

### **Architecture & Patterns**
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling and testability
- **DTO Pattern** - Data transfer objects for API contracts
- **Extension Methods** - Clean DI registration

### **Development Tools**
- **Visual Studio 2022** / **Visual Studio Code**
- **Swagger/OpenAPI** - API documentation and testing
- **Git** - Version control

---

## 🎨 Frontend Application

The TaskManager frontend is a modern, responsive web application built with **React.js**.

### **Repository**
🔗 **[Task-Manager-Frontend](https://github.com/Muhammad-HuzaifaBaig/Task-Manager-Frontend)**

### **Tech Stack**
- **React.js** - Modern JavaScript library for building user interfaces
- **React Router** - Client-side routing
- **Tailwind CSS** - Utility-first CSS framework
- **Shadcn/UI** - Re-usable component library
- **Lucide React** - Icon library
- **Vite** - Fast build tool and dev server

### **Features**
- 🎯 **Modern UI/UX** - Clean, intuitive interface with smooth animations
- 🔐 **Authentication** - Login and signup with JWT token management
- 📊 **Dashboard** - Real-time task statistics and progress tracking
- ✅ **Task Management** - Create, update, delete, and filter tasks
- 👤 **User Profile** - View profile and task statistics
- 🎨 **Responsive Design** - Mobile-friendly and adaptive layout
- 🌙 **Role-Based UI** - Dynamic interface based on user role (Admin/Normal User)

### **Quick Start**
```bash
# Clone the frontend repository
git clone https://github.com/Muhammad-HuzaifaBaig/Task-Manager-Frontend.git
cd Task-Manager-Frontend

# Install dependencies
npm install

# Start development server
npm run dev
```

> 💡 Make sure the backend API is running before starting the frontend application.

---

## 🚀 Getting Started

### **Prerequisites**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or Developer Edition)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### **Installation**

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/Task-Manager-Backend.git
   cd Task-Manager-Backend
   ```

2. **Configure User Secrets**
   ```bash
   cd TaskManager.Api
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TaskManager;User ID=sa;Password=YourPassword;TrustServerCertificate=True;"
   dotnet user-secrets set "JwtSettings:SecretKey" "YourSuperSecretKeyMinimum32Characters!"
   ```

   > 💡 See [SECURITY.md](./SECURITY.md) for detailed security configuration

3. **Update Database**
   ```bash
   cd TaskManager.Data
   dotnet ef database update
   ```

4. **Build the solution**
   ```bash
   cd ..
   dotnet build
   ```

5. **Run the API**
   ```bash
   cd TaskManager.Api
   dotnet run
   ```

   The API will be available at:
   - **HTTP**: `http://localhost:5000`
   - **HTTPS**: `https://localhost:5001`
   - **Swagger**: `https://localhost:5001/swagger`

---

## 📁 Project Structure

```
Task-Manager-Backend/
│
├── TaskManager.Api/                 # Presentation Layer
│   ├── Controllers/
│   │   └── AuthController.cs        # Authentication endpoints
│   ├── Program.cs                   # Application entry point
│   ├── appsettings.json            # Non-sensitive configuration
│   └── TaskManager.Api.csproj
│
├── TaskManager.Business/            # Business Logic Layer
│   ├── Services/
│   │   └── Auth/
│   │       ├── IAuthService.cs     # Service interface
│   │       └── AuthService.cs      # Service implementation
│   ├── Helpers/
│   │   ├── IJwtTokenGenerator.cs   # JWT helper interface
│   │   └── JwtTokenGenerator.cs    # JWT token generation
│   ├── BusinessLayer.cs            # DI registration
│   └── TaskManager.Business.csproj
│
├── TaskManager.Data/                # Data Access Layer
│   ├── Repositories/
│   │   └── Auth/
│   │       ├── IAuthRepository.cs  # Repository interface
│   │       └── AuthRepository.cs   # Repository implementation
│   ├── TaskManagerDbContext.cs     # EF Core DbContext
│   ├── DataLayer.cs                # DI registration
│   └── TaskManager.Data.csproj
│
├── TaskManager.Model/               # Domain Models & DTOs
│   └── Model/
│       ├── DataLayer/              # Database entities
│       │   ├── User.cs
│       │   ├── Task.cs
│       │   ├── Role.cs
│       │   └── ...
│       ├── BusinessLayer/
│       │   ├── RequestDTO/         # API request models
│       │   │   ├── LoginRequestDTO.cs
│       │   │   └── SignupRequestDTO.cs
│       │   └── ResponseDTO/        # API response models
│       │       └── LoginResponseDTO.cs
│       ├── Common/
│       │   └── ResponseMessage.cs  # Standardized API response
│       └── Enum/
│           └── eRole.cs            # Role enumeration
│
├── .gitignore                      # Git ignore rules
├── README.md                       # This file
├── SECURITY.md                     # Security documentation
└── TaskManager.sln                 # Solution file
```

---

## 🌐 API Endpoints

### **Authentication**

#### **POST** `/api/auth/signup`
Register a new user account.

**Request:**
```json
{
  "fullName": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**Success Response (201 Created):**
```json
{
  "success": true,
  "data": null,
  "message": "Signup successful. Please login to continue.",
  "statusCode": 201
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "data": null,
  "message": "Email already exists",
  "statusCode": 400
}
```

---

#### **POST** `/api/auth/login`
Authenticate user and receive JWT token.

**Request:**
```json
{
  "username": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 5,
    "email": "john.doe@example.com",
    "fullName": "John Doe",
    "roleId": 2,
    "expiresAt": "2026-01-14T18:30:00Z"
  },
  "message": "Login successful",
  "statusCode": 200
}
```

**Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "data": null,
  "message": "Invalid username or password",
  "statusCode": 401
}
```

---

### **Response Format**

All API responses follow this standardized format:

```json
{
  "success": true | false,
  "data": <object> | null,
  "message": "string",
  "statusCode": 200 | 201 | 400 | 401 | 404 | 500
}
```

**HTTP Status Codes:**
- `200` - OK (Success)
- `201` - Created (Resource created)
- `400` - Bad Request (Validation error)
- `401` - Unauthorized (Authentication failed)
- `404` - Not Found (Resource not found)
- `500` - Internal Server Error (Server error)

---

## 🔒 Security

### **Authentication Flow**
1. User signs up → Password hashed with BCrypt → Stored in database
2. User logs in → Password verified → JWT token generated
3. User accesses protected endpoints → JWT token validated
4. Token expires after 60 minutes (configurable)

### **Password Security**
- ✅ BCrypt hashing with salt
- ✅ Minimum 8 characters recommended
- ✅ Never stored in plain text
- ✅ Secure password verification

### **JWT Token**
- ✅ HMAC SHA256 signing
- ✅ Contains user claims (UserId, Email, Role)
- ✅ Configurable expiration time
- ✅ Stateless authentication

### **Credential Management**
- ✅ User Secrets for local development
- ✅ Environment variables for production
- ✅ No secrets in source code
- ✅ See [SECURITY.md](./SECURITY.md) for details

---

## ⚙️ Configuration

### **appsettings.json** (Non-sensitive)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManager;Integrated Security=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "",
    "Issuer": "TaskManagerAPI",
    "Audience": "TaskManagerClient",
    "ExpirationMinutes": 60
  }
}
```

### **User Secrets** (Sensitive - Local only)
```bash
# View secrets
dotnet user-secrets list

# Set secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Your-Connection-String"
dotnet user-secrets set "JwtSettings:SecretKey" "Your-Secret-Key"
```

### **Environment Variables** (Production)
```bash
# Windows
setx ConnectionStrings__DefaultConnection "Production-Connection-String"
setx JwtSettings__SecretKey "Production-Secret-Key"

# Linux/Mac
export ConnectionStrings__DefaultConnection="Production-Connection-String"
export JwtSettings__SecretKey="Production-Secret-Key"
```

---

## 💻 Development Guidelines

### **Code Style**
- Follow C# naming conventions (PascalCase for classes, camelCase for locals)
- Use meaningful variable names
- Keep methods small and focused (Single Responsibility Principle)
- Add XML comments for public APIs

### **Git Workflow**
```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Commit changes
git add .
git commit -m "feat: add user authentication"

# Push changes
git push origin feature/your-feature-name

# Create Pull Request
```

### **Commit Message Convention**
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `refactor:` - Code refactoring
- `test:` - Adding tests
- `chore:` - Maintenance tasks

### **Adding New Features**

1. **Create Model** (TaskManager.Model)
2. **Create Repository Interface & Implementation** (TaskManager.Data)
3. **Register Repository** in `DataLayer.cs`
4. **Create Service Interface & Implementation** (TaskManager.Business)
5. **Register Service** in `BusinessLayer.cs`
6. **Create Controller** (TaskManager.Api)
7. **Test** with Swagger/Postman

---

## 🧪 Testing

### **Testing with Swagger**
1. Run the application
2. Navigate to `https://localhost:5001/swagger`
3. Test endpoints directly from the UI

### **Testing with Postman**
Import the collection and test all endpoints with different scenarios.

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT.io](https://jwt.io/) - JWT debugger
- [BCrypt Guide](https://github.com/BcryptNet/bcrypt.net)

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

**Huzaifa Baig**

---

## 🙏 Acknowledgments

- .NET Team for the amazing framework
- Community contributors and best practices

---

## 📞 Support

For issues, questions, or contributions:
- 📧 Email: your.email@example.com
- 🐛 Issues: [GitHub Issues](https://github.com/yourusername/Task-Manager-Backend/issues)

---

**Built with ❤️ using .NET 8 and best practices**
