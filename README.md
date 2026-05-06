# TipJarBackend
A high-performance, secure backend built to help service industry professionals track their earnings. This API calculates total, monthly, and yearly tips while utilizing caching to minimize database overhead.

## 🚀 Key Features
*   **Earnings Analytics:** Real-time calculation of daily, monthly, and annual tip totals.
*   **Secure Authentication:** JWT-based identity management for complete user data isolation.
*   **Performance First:** Implemented Redis caching to reduce database "chatter" and latency.
*   **Scalable Core:** Built using Clean Architecture and Dependency Injection for easy maintenance.

## ⚡ Performance Optimization: Cache-Aside Pattern
To ensure the app stays fast and Supabase usage remains efficient, I implemented a **Redis** caching layer:
*   **Read Strategy:** The API first checks Redis for cached user data. If found (Cache Hit), it returns immediately. If not (Cache Miss), it fetches from Supabase and populates the cache.
*   **Write Strategy (Invalidation):** Any CRUD operation (adding, editing, or deleting a tip) automatically triggers a cache deletion. This ensures the user always sees fresh data on their next fetch without manual refreshing.

## 🛠 Tech Stack
*   **Language/Framework:** .NET 10 (C#)
*   **Database:** Supabase (PostgreSQL)
*   **ORM:** Entity Framework Core
*   **Caching:** Redis
*   **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API)
*   **Auth:** JWT (JSON Web Tokens)

## 🏗 Project Structure
The solution follows **Clean Architecture** to ensure the code remains scalable and decoupled:

### 1. Domain
*   **Entities:** Core business objects (User, Tip entries).
*   **Interfaces:** Abstractions and contracts for repositories and services.

### 2. Application
*   **Services:** Implementation of business logic and cache orchestration.
*   **DTOs & ReadModels:** Data transfer objects and optimized models for the UI.
*   **Mappings:** Object-to-object mapping logic.
*   **Custom Exceptions:** Application-specific error types.

### 3. Infrastructure
*   **Data:** EF Core implementation and `AppDbContext`.
*   **Repositories:** Concrete implementations of data persistence.
*   **Security:** Password hashing and encryption utilities.

### 4. API
*   **Controllers:** RESTful endpoints for the frontend.
*   **Middleware:** Global exception handling to catch and format application errors.

## 🔧 Getting Started

### Prerequisites
*   .NET 10 SDK
*   Redis server (local or cloud)
*   Supabase account

### Installation & Setup
1. **Clone the repo:**
   ```bash
   git clone [https://github.com/yourusername/tiptracker-backend.git](https://github.com/yourusername/tiptracker-backend.git)
   
2. **Configure .env**
3. **Migrate Database**
4. **Run Application**

## 🔗 Related
