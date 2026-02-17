# Task Manager

A full-stack Task Manager application.

## 🔗 Frontend

👉 [Task Manager Frontend](https://github.com/Muhammad-HuzaifaBaig/Task-Manager-Frontend)

---
## 🛠️ Backend Setup Guide

### Prerequisites

Make sure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [EF Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

Install EF Core CLI if you don't have it:
```bash
dotnet tool install --global dotnet-ef
```
---

### 1. Clone the Repository

```bash
git clone https://github.com/Muhammad-HuzaifaBaig/Task-Manager-One.git
cd Task-Manager-One
```
---

### 2. Update Connection String

Open `appsettings.json` and update the connection string with your SQL Server details:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TaskManager;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g. `localhost` or `DESKTOP-XXXX\SQLEXPRESS`).

---

### 3. Run Migrations

This will automatically create the `TaskManager` database and all tables:

```bash
dotnet ef database update
```
---

### 4. Run the Project

```bash
dotnet run
```

The API will start running. You can test it via Swagger at:
```
https://localhost:{port}/swagger
```
---

### ✅ That's it!

The database will be created automatically — no manual SQL scripts needed.
