# Wrocław Theatre Tickets - Backend

A modern .NET 10 Clean Architecture REST API for aggregating and managing theatre events in Wrocław with search, filtering, user accounts, reviews, and notifications.

## 🎯 Project Overview

This backend application provides a comprehensive platform for:
- **Theater event aggregation** from multiple Wrocław theatres
- **Advanced search and filtering** by type, date, price, age restriction, language
- **User management** with JWT authentication and OAuth support
- **Social features** (favorites, reviews, ratings)
- **Notification system** for event reminders and digests
- **Admin panel** for content moderation and analytics

## 🏗️ Architecture

**Clean Architecture** with strict separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│                    Web API (Minimal APIs)           │
│          (Controllers, Endpoints, Middleware)        │
└─────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────┐
│            Application Layer (Use Cases)             │
│    (MediatR Commands/Queries, DTOs, Validators)     │
└─────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────┐
│        Infrastructure Layer (Implementations)       │
│    (Repositories, Services, Database Context)       │
└─────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────┐
│         Domain Layer (Business Logic)                │
│     (Entities, Value Objects, Domain Services)      │
└─────────────────────────────────────────────────────┘
```

## 📋 Features

### ✅ Implemented
- [x] Clean Architecture with 4-layer separation
- [x] Entity Framework Core with SQLite
- [x] JWT Bearer authentication
- [x] OAuth support (Google, Facebook ready)
- [x] Role-based access control (User, Moderator, Admin)
- [x] Advanced search and filtering
- [x] User favorites management
- [x] Review system with ratings
- [x] Notification framework (email/push)
- [x] Admin review approval workflow
- [x] Web scraping framework (HtmlAgilityPack)
- [x] Structured logging (Serilog)
- [x] FluentValidation for inputs
- [x] AutoMapper for DTOs
- [x] CORS support
- [x] Swagger/OpenAPI documentation

### 📋 In Progress/Planned
- [ ] Scheduled theater website parsing (Quartz.NET)
- [ ] Email notification service
- [ ] Push notification integration
- [ ] Admin dashboard endpoints
- [ ] Calendar export (iCal, Google Calendar)
- [ ] Performance caching (Redis)
- [ ] Full-text search (Elasticsearch)
- [ ] GraphQL API alternative

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK
- VSCode, Visual Studio, or Rider
- SQLite (included with .NET)

### Installation

1. **Clone the repository**
```bash
cd d:\Git\WroclawTheatreTickets
```

2. **Restore dependencies**
```powershell
dotnet restore
```

3. **Build the solution**
```powershell
dotnet build WroclawTheatreTickets.slnx
```

4. **Run the application**
```powershell
cd src/WroclawTheatreTickets.Web
dotnet run
```

5. **Access the API**
- API: `https://localhost:5001/api`
- Swagger UI: `https://localhost:5001/swagger` (Development only)

## 📚 API Documentation

### Shows
```
GET    /api/shows              - Get all active shows
GET    /api/shows/{id}         - Get show details with reviews
GET    /api/shows/upcoming     - Get shows in next N days
GET    /api/shows/search       - Search by keyword
POST   /api/shows/filter       - Advanced filtering
GET    /api/shows/trending/viewed - Most viewed shows
```

### Authentication
```
POST   /api/auth/register      - Register new user
POST   /api/auth/login         - Login with email/password
POST   /api/auth/oauth         - Login with OAuth provider
```

### Favorites (Auth Required)
```
GET    /api/favorites          - Get user's favorites
POST   /api/favorites/{showId} - Add to favorites
DELETE /api/favorites/{showId} - Remove from favorites
```

### Reviews
```
POST   /api/reviews            - Create review (Auth required)
GET    /api/reviews/show/{id}  - Get show reviews
```

### Admin (Admin Role Required)
```
POST   /api/admin/reviews/{id}/approve - Approve review
```

## 🔐 Authentication

### JWT Token Usage
```bash
# Get token from login/register response
curl -X GET "https://localhost:5001/api/shows" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Token Structure
```json
{
  "sub": "user-id",
  "email": "user@example.com",
  "role": "User",
  "exp": "2025-02-09T15:30:00Z"
}
```

## 📊 Database Schema

### Main Tables
- **Theatres** - Theater information
- **Shows** - Theater performances/events
- **Users** - User accounts
- **UserFavorites** - Bookmarked shows
- **Reviews** - User reviews
- **ViewHistory** - User activity tracking
- **Notifications** - User notifications

See [BACKEND_SUMMARY.md](./BACKEND_SUMMARY.md) for detailed schema.

## 🛠️ Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **API** | ASP.NET Core 10 | RESTful API |
| **Patterns** | MediatR | CQRS pattern |
| **Database** | EF Core 10 + SQLite | ORM & data access |
| **Authentication** | JWT Bearer | Stateless auth |
| **Validation** | FluentValidation | Input validation |
| **Mapping** | AutoMapper | DTO mapping |
| **Logging** | Serilog | Structured logging |
| **HTML Parsing** | HtmlAgilityPack | Web scraping |
| **Scheduling** | Quartz.NET | Job scheduling (planned) |
| **Password** | BCrypt.Net | Secure hashing |

## 📁 Project Structure

```
WroclawTheatreTickets/
├── src/
│   ├── WroclawTheatreTickets.Domain/          # Business logic
│   │   ├── Common/
│   │   └── Entities/
│   ├── WroclawTheatreTickets.Application/     # Use cases
│   │   ├── Contracts/
│   │   ├── UseCases/
│   │   ├── Mapping/
│   │   └── Validators/
│   ├── WroclawTheatreTickets.Infrastructure/  # Data access
│   │   ├── Data/
│   │   ├── Repositories/
│   │   └── Services/
│   └── WroclawTheatreTickets.Web/            # API Layer
│       ├── Program.cs
│       ├── ServiceCollectionExtensions.cs
│       ├── Endpoints.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
├── BACKEND_SUMMARY.md                        # Architecture details
├── QUICK_START.md                            # Getting started
├── ARCHITECTURE_DECISIONS.md                 # Design decisions
└── WroclawTheatreTickets.slnx               # Solution file
```

## 🔧 Configuration

### JWT Secret
Edit `appsettings.json`:
```json
"Jwt": {
  "Secret": "your-secret-at-least-32-characters-long",
  "Issuer": "WroclawTheatreTickets",
  "Audience": "WroclawTheatreTicketsUsers"
}
```
⚠️ **IMPORTANT**: Change in production!

### Database
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=theatre.db"
}
```

### Email Service
```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "From": "noreply@example.com",
  "Password": "your-app-password"
}
```

## 📖 Documentation

- [Backend Summary](./BACKEND_SUMMARY.md) - Complete feature overview
- [Quick Start Guide](./QUICK_START.md) - API examples and commands
- [Architecture Decisions](./ARCHITECTURE_DECISIONS.md) - Design rationale

## 🧪 Testing

Unit and integration tests (framework prepared, tests to be added):

```powershell
dotnet test
```

## 📈 Performance

- **Async/Await**: All I/O operations are non-blocking
- **Indexes**: Database indexes on frequently queried fields
- **Eager Loading**: Strategic use of Include() for related entities
- **DTO Mapping**: AutoMapper with efficient configuration
- **Structured Logging**: Async Serilog doesn't block requests

## 🔒 Security

- ✅ Password hashing with BCrypt
- ✅ JWT token-based authentication
- ✅ Role-based authorization
- ✅ HTTPS enforcement (in production)
- ✅ CORS configuration
- ✅ Input validation
- ⚠️ TODO: Rate limiting
- ⚠️ TODO: Request sanitization

## 📝 Logging

Serilog structured logging to:
- Console (development)
- Rolling file logs in `logs/` directory (production)
- Can be extended to: Seq, Splunk, ELK, DataDog

Access logs:
```
logs/app20250209.txt
logs/app20250210.txt
...
```

## 🐛 Troubleshooting

### Port in use
Change port in `appsettings.json`:
```json
"Kestrel": {
  "Endpoints": {
    "Https": {
      "Url": "https://localhost:5002"
    }
  }
}
```

### Database errors
```powershell
rm theatre.db
dotnet run
```

### Build failures
```powershell
dotnet clean
dotnet restore
dotnet build
```

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Use meaningful commit messages
3. Add tests for new features
4. Update documentation

## 📄 License

(Add your license information)

## 👥 Team

- Architecture & Backend: Your Name

## 📞 Support

For issues, feature requests, or questions:
1. Check documentation files
2. Review Architecture Decisions
3. Create an issue in the repository

## 🎓 Learning Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

**Status**: ✅ **READY FOR DEVELOPMENT**

Built with ❤️ for Wrocław Theatre Lovers

Last Updated: February 9, 2025
