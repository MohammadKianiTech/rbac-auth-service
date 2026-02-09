# Evaluation & Auth API

A minimal yet production-ready .NET 9 Web API for managing **evaluations**, **users**, **roles**, and **fine-grained permissions**, built using industry-standard architecture and security practices.

---

## 🛠 Tech Stack

![stack](https://skillicons.dev/icons?i=dotnet,cs,postgres,redis,serilog)

- **.NET 9**
- **C#**
- **PostgreSQL** (primary database)
- **Redis** (for caching and JWT token invalidation)
- **Clean Architecture** (separation of concerns for testability and maintainability)
- **CQRS with MediatR** (command/query segregation)
- **JWT-based Authentication & Role/Permission Authorization**
- **Serilog** (structured logging)

---

## 🚀 Quick Start

Ensure you have **Docker** and **Docker Compose** installed.

### 1. Run the application with seeded data

```bash
docker compose up -d --build
```

This command will:

- Start PostgreSQL container.
- Apply EF Core migrations .
- Run Redis as a distributed cache for performance and session management.
- Launch Seq for structured event logging and real-time log inspection.
- Seed initial data, including an admin user.

### 2. Default Admin Credentials

After the containers are up, authenticate using:

- **Email**: `admin@aawiz.com`
- **Password**: `123456`

Use these credentials to request a JWT token from the authentication endpoint and access protected APIs.

### 3. Open Swagger
```bash
http://localhost:5000/swagger/index.html
```

### 4. Check webservice logs
```bash
http://localhost:8081/#/login
```
- **Username**: `admin`
- **Password**: `Admin123!`
After login please set a new password.