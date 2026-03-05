# Contoso Pizza Full-Stack App

A full-stack pizza ordering system built with ASP.NET Core Web API, EF Core, SQL Server in Docker, and a React frontend.  
Demonstrates CRUD operations, JWT authentication, API versioning, and order status computation.

---

## Tech Stack

**Backend:**  
- ASP.NET Core 10 Web API  
- Entity Framework Core with SQL Server  
- JWT Authentication  
- API Versioning (v1, v2)  
- Dockerized SQL Server  

**Frontend:**  
- React (JavaScript)  
- Axios for API requests  

---

## Features

- Orders CRUD: Create, read, update, delete pizza orders  
- Order Status: Computed mock status based on pickup time  
- API Versioning: Supports v1 and v2 endpoints  
- JWT Authentication: Secure login with hashed passwords  
- Swagger UI: Interactive API documentation  
- Dockerized Database: SQL Server container for easy setup  

---

## Project Structure

contoso-pizza-fullstack/

├─ backend/ # ASP.NET Core API

├─ frontend/ # React app

└─ README.md

---

## Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js & npm
- Docker Desktop

---

### Backend Setup

1. Navigate to the backend folder:

```bash
cd backend
```
2. Restore dependencies:
```bash
dotnet restore
```
3. Apply EF migrations and create the database:
```bash
dotnet ef database update
```
4. Run the backend:
```bash
dotnet run
```
5. API will be available at http://localhost:5000

# Frontend Setup
1. Navigate to the frontend folder:
```bash
cd frontend
```
2. Install dependencies:
```bash
npm install
```
3. Run the React app:
```bash
npm start
```
4. Frontend will open at http://localhost:3000 and communicate with the backend API.

# Authentication
Use the login endpoint (/api/auth/login) to get a JWT token.
Include the token in the Authorization: Bearer <token> header for protected endpoints.

# Notes
Database credentials are stored in user secrets; make sure not to commit them.
The backend computes mock order status for demonstration purposes.

# License
This project is for portfolio and demonstration purposes only.
