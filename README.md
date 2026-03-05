# Contoso Pizza Full-Stack App (GraphQL Edition)

A full-stack pizza ordering system built with ASP.NET Core Web API, EF Core, SQL Server in Docker, and a React frontend.
Now demonstrates GraphQL queries and mutations alongside CRUD operations, JWT authentication, and order status computation.

---

## Tech Stack

**Backend:**  
- ASP.NET Core 10 Web API  
- Entity Framework Core with SQL Server  
- JWT Authentication
- GraphQL via HotChocolate
- API Versioning (v1, v2)  
- Dockerized SQL Server  

**Frontend:**  
- React (JavaScript)  
- Axios for API requests  

---

## Features

- Orders CRUD: Create, read, update, delete pizza orders via GraphQL
- Pizza & Toppings Queries: Retrieve pizzas with toppings, filter by properties (e.g., gluten-free)
- Order Status: Computed mock status based on pickup time
- JWT Authentication: Secure login with hashed passwords
- Role-Based Authorization: Admins can create pizzas; users can create orders
- GraphQL Playground: Interactive queries & mutations at /gql
- Dockerized Database: SQL Server container for easy setup

---

## Project Structure

contoso-pizza-graphql/

├─ backend/ # ASP.NET Core API with GraphQL

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
5. GraphQL Playground will be available at:
```bash
http://localhost:5000/gql
```

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
4. Frontend will open at http://localhost:3000 and communicate with the GraphQL API.

# Authentication
Use the login endpoint (/api/auth/login) to get a JWT token.
Include the token in the Authorization: Bearer <token> header for protected endpoints.

# Notes
Database credentials are stored in user secrets; make sure not to commit them.
The backend computes mock order status for demonstration purposes.

# License
This project is for portfolio and demonstration purposes only.
