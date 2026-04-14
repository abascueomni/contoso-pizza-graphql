# Contoso Pizza Full-Stack App (GraphQL & Redis Edition)

A full-stack pizza ordering system built with ASP.NET Core Web API, EF Core, SQL Server in Docker, and a React frontend.

The project demonstrates GraphQL queries and mutations, JWT authentication, and a cache-aside caching pattern using interchangeable implementations via `IDistributedCache`.

Redis-based distributed caching is implemented in a dedicated feature branch using Docker Redis for local development.

---

## Tech Stack

**Backend:**  
- ASP.NET Core 10 Web API  
- Entity Framework Core with SQL Server  
- JWT Authentication
- GraphQL via HotChocolate
- Dockerized SQL Server
- `IDistributedCache` caching abstraction

**Caching:**
- Main branch: In-memory distributed caching (`AddDistributedMemoryCache`)
- Redis branch: Redis distributed caching via Docker (`AddStackExchangeRedisCache`)
- Cache-aside pattern with manual invalidation and hit/miss logging

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
- Cache-aside pattern implemented using `IDistributedCache` with interchangeable providers (in-memory for main branch, Redis for feature branch)
- Cache Performance Tracking: Track hits, misses
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

## Backend Setup (Main Branch - No Redis Required)

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
## Backend Setup (Redis Branch - Requires Docker Redis)
1. Start Redis:
```bash
docker run -d -p 6379:6379 redis
```
2. Run backend
```bash
dotnet run
```
This branch enables Redis-based distributed caching via IDistributedCache.

## Frontend Setup
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
