# Wrocław Theatre Tickets - Backend

A production-ready .NET 10 Clean Architecture REST API for aggregating and managing theatre events in Wrocław with search, filtering, user accounts, reviews, and automated synchronization.

## 🎯 Project Overview

This backend application provides a comprehensive platform for:
- **Automated theater event aggregation** from multiple Wrocław theatres using scheduled jobs
- **Advanced search and filtering** by type, date, price, age restriction, language, and theatre
- **Secure user management** with JWT authentication and OAuth framework (Google, Facebook)
- **Social features** including favorites, reviews (1-5 stars), and user ratings
- **Notification system** for event reminders, digests, and alerts
- **Admin moderation panel** for review approval and content management
- **Automated maintenance** with scheduled cleanup jobs and data synchronization

## 🏗️ Architecture

**Clean Architecture** with strict separation of concerns and dependency inversion:

```
┌─────────────────────────────────────────────────────┐
│              Web API (Minimal APIs)                 │
│        Endpoints, Authentication, Logging           │
└─────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────┐
│         Application Layer (Use Cases)               │
│   MediatR CQRS, DTOs, Validators, Mapping          │
└─────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────┐
│      Infrastructure Layer (Implementations)         │
│  Repositories, Services, Jobs, Database Context     │
└─────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────┐
│        Domain Layer (Business Logic)                │
│    Entities, Value Objects, Domain Services         │
└─────────────────────────────────────────────────────┘
```

## 📋 Features

### ✅ Fully Implemented
- [x] Clean Architecture with 4-layer separation (Domain, Application, Infrastructure, Web)
- [x] Entity Framework Core 10 with SQLite
- [x] JWT Bearer authentication with BCrypt password hashing
- [x] OAuth support framework (Google, Facebook ready for configuration)
- [x] Role-based authorization (User, Moderator, Admin)
- [x] Advanced search and filtering with multiple criteria
- [x] User favorites management (add/remove/list)
- [x] Review system with ratings (1-5 stars) and approval workflow
- [x] Notification framework (email/push infrastructure ready)
- [x] Admin review approval endpoints
- [x] Theatre repertoire synchronization service
- [x] Scheduled background jobs (Quartz.NET):
  - Daily theatre website synchronization (2:00 AM)
  - Weekly cleanup of old shows (Sunday 3:00 AM)
- [x] Structured logging with Serilog (console + file rotation)
- [x] FluentValidation for input validation
- [x] AutoMapper for DTO mapping
- [x] CORS support
- [x] Swagger/OpenAPI documentation
- [x] Comprehensive test suite (66 tests, 100% passing)

### 🚧 Ready for Integration (Stubs/Frameworks in Place)
- [ ] Email notification service (SMTP configured, awaiting credentials)
- [ ] Push notification integration (framework ready for Firebase/OneSignal)
- [ ] Theatre website parsers (HtmlAgilityPack integrated, site-specific parsers needed)
- [ ] OAuth provider tokens (endpoints exist, provider config needed)

### 📋 Future Enhancements
- [ ] Additional admin dashboard endpoints
- [ ] Calendar export (iCal, Google Calendar)
- [ ] Performance caching layer (Redis)
- [ ] Full-text search (Elasticsearch/Lucene)
- [ ] GraphQL API alternative
- [ ] Rate limiting

## 🚀 Quick Start

### Prerequisites
- **.NET 10 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **IDE**: Visual Studio 2025, VS Code, or JetBrains Rider
- **SQLite** (included with .NET, no separate install needed)
- **PowerShell** (for Windows) or Bash (for Linux/macOS)

### Installation Steps

1. **Clone and navigate to repository**
```powershell
cd d:\Git\WroclawTheatreTickets
```

2. **Restore NuGet packages**
```powershell
dotnet restore
```

3. **Build the solution**
```powershell
dotnet build WroclawTheatreTickets.slnx -c Release
```

4. **Run tests (optional but recommended)**
```powershell
dotnet test
# Expected: 66 tests passed (100% success rate)
```

5. **Launch the application**
```powershell
cd src\WroclawTheatreTickets.Web
dotnet run
```

Or with **force synchronization** on startup:
```powershell
dotnet run --force-sync
```

6. **Access the API**
- **API Base URL**: `https://localhost:5001/api`
- **Swagger UI**: `https://localhost:5001/swagger` (Development only)
- **Health Check**: `https://localhost:5001/api/shows`

## 📚 API Documentation

### Shows (Public Endpoints)
```http
GET    /api/shows                      # Get all active shows
GET    /api/shows/{id}                 # Get show details with reviews
GET    /api/shows/upcoming?days=30     # Get shows in next N days (default: 30)
GET    /api/shows/search?keyword=opera # Search by keyword (title/cast/director)
POST   /api/shows/filter               # Advanced filtering (see below)
GET    /api/shows/trending/viewed      # Most viewed shows (sorted by viewCount)
```

**Filter Request Body Example:**
```json
{
  "types": ["Musical", "Opera"],
  "minPrice": 50,
  "maxPrice": 200,
  "ageRestriction": "12+",
  "language": "POL",
  "startDate": "2026-02-15T00:00:00Z",
  "endDate": "2026-03-15T00:00:00Z",
  "theatreId": "guid-here"
}
```

### Authentication (Public Endpoints)
```http
POST   /api/auth/register              # Register new user
POST   /api/auth/login                 # Login with email/password
POST   /api/auth/oauth                 # Login with OAuth provider (Google/Facebook)
```

**Registration Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "Jan",
  "lastName": "Kowalski"
}
```

**Login Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response (all auth endpoints):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "Jan",
    "lastName": "Kowalski",
    "role": "User"
  }
}
```

### Favorites (🔐 Authentication Required)
```http
GET    /api/favorites                  # Get user's favorite shows
POST   /api/favorites/{showId}         # Add show to favorites
DELETE /api/favorites/{showId}         # Remove show from favorites
```

### Reviews
```http
POST   /api/reviews                    # Create review (🔐 Auth required)
GET    /api/reviews/show/{showId}      # Get all reviews for a show (public)
```

**Create Review Request:**
```json
{
  "showId": "guid",
  "rating": 5,
  "comment": "Amazing performance!"
}
```

### Admin (🔐 Admin Role Required)
```http
POST   /api/admin/reviews/{reviewId}/approve  # Approve pending review
```

## 🔐 Authentication & Authorization

### JWT Token Usage

1. **Obtain token** by calling `/api/auth/register` or `/api/auth/login`
2. **Include in requests** using the `Authorization` header:

```bash
curl -X GET "https://localhost:5001/api/favorites" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Token Structure
The JWT contains:
```json
{
  "sub": "user-guid",
  "email": "user@example.com", 
  "role": "User",
  "exp": 1738943400
}
```

### Role Hierarchy
- **User** - Default role, can manage favorites, create reviews
- **Moderator** - Can approve reviews (planned)
- **Admin** - Full access, can approve reviews and manage content

## 📊 Database Schema

### Core Tables

**Theatres** - Theatre venue information
- Basic info: name, address, city, phone, email, website
- Booking: bookingUrl, ticketUrl
- Location: latitude, longitude (ready for map integration)
- Status: isActive flag

**Shows** - Theatre performances and events
- Core: title, description, fullDescription, type (enum)
- Relations: theatreId (FK)
- People: director, cast
- Schedule: startDateTime, endDateTime, duration, language
- Pricing: minPrice, maxPrice, ageRestriction (enum)
- Media: posterUrl, imageUrl, ticketUrl
- Analytics: viewCount, rating (calculated), reviewCount
- Sync: externalId (for deduplication), isActive

**Users** - User accounts
- Auth: email (unique), passwordHash, provider, externalId, isEmailVerified
- Profile: firstName, lastName, role (enum)
- Settings: enableEmailNotifications, enablePushNotifications, preferredCategories (JSON)
- Tracking: lastLoginAt, isActive

**UserFavorites** - User's bookmarked shows
- userId (FK), showId (FK)
- Unique constraint on (userId + showId)

**Reviews** - User ratings and comments
- userId (FK), showId (FK)
- rating (1-5), comment
- isApproved (moderation flag)
- timestamps (createdAt, updatedAt)

**ViewHistory** - User activity tracking
- userId (FK), showId (FK), viewedAt
- Used for trending/recommendations

**Notifications** - User notification queue
- userId (FK), showId (FK, nullable)
- title, message, type (enum)
- isRead, createdAt

### Performance Indexes
- `Theatre.Name`, `Theatre.IsActive`
- `Show.Title`, `Show.StartDateTime`, `Show.TheatreId`, `Show.IsActive`, `Show.ExternalId`
- `User.Email` (unique), `User.ExternalId` + `User.Provider`
- `UserFavorite.UserId` + `UserFavorite.ShowId` (unique composite)
- `Review.ShowId` + `Review.IsApproved`, `Review.UserId`
- `ViewHistory.UserId` + `ViewHistory.ViewedAt DESC`
- `Notification.UserId` + `Notification.IsRead`, `Notification.CreatedAt DESC`

## 🛠️ Technology Stack

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| **Framework** | ASP.NET Core | 10.0 | RESTful Web API |
| **Pattern** | MediatR | 12.4.0 | CQRS (Command Query Separation) |
| **Database** | EF Core + SQLite | 10.0.0 | ORM & lightweight data storage |
| **Authentication** | JWT Bearer | 10.0.0 | Stateless token-based auth |
| **Password** | BCrypt.Net-Next | 4.0.3 | Secure password hashing |
| **Validation** | FluentValidation | 11.9.2 | Input validation rules |
| **Mapping** | AutoMapper | 12.0.1 | DTO ↔ Entity mapping |
| **Logging** | Serilog | 4.0.0 | Structured logging (console + file) |
| **Scheduling** | Quartz.NET | 3.13.0 | Background job scheduling |
| **Web Scraping** | HtmlAgilityPack | 1.11.65 | HTML parsing for theatre sites |
| **API Docs** | Swashbuckle | 7.2.0 | OpenAPI/Swagger generation |
| **Testing** | xUnit + Moq | Latest | Unit & integration tests |

### External Dependencies
```xml
<!-- Core Framework -->
Microsoft.AspNetCore.App (10.0)
Microsoft.EntityFrameworkCore (10.0)
Microsoft.EntityFrameworkCore.Sqlite (10.0)

<!-- Authentication & Security -->
Microsoft.AspNetCore.Authentication.JwtBearer (10.0)
System.IdentityModel.Tokens.Jwt (8.2.0)
BCrypt.Net-Next (4.0.3)

<!-- Application Patterns -->
MediatR (12.4.0)
AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
FluentValidation.DependencyInjectionExtensions (11.9.2)

<!-- Infrastructure -->
Quartz.Extensions.Hosting (3.13.0)
HtmlAgilityPack (1.11.65)
Serilog.AspNetCore (10.0.0)
```

## 📁 Project Structure

```
WroclawTheatreTickets/
├── src/
│   ├── WroclawTheatreTickets.Domain/          # Business Logic Layer (no dependencies)
│   │   ├── Common/
│   │   │   ├── Entity.cs                      # Base entity with audit fields
│   │   │   ├── ValueObject.cs                 # Immutable value objects
│   │   │   └── Result.cs                      # Result pattern implementation
│   │   └── Entities/
│   │       ├── Theatre.cs                     # Theatre venue entity
│   │       ├── Show.cs                        # Theatre show/performance entity
│   │       ├── User.cs                        # User account entity
│   │       ├── UserFavorite.cs                # User favorites relationship
│   │       ├── Review.cs                      # User reviews with ratings
│   │       ├── ViewHistory.cs                 # Show view tracking
│   │       └── Notification.cs                # User notifications
│   │
│   ├── WroclawTheatreTickets.Application/     # Application Logic Layer
│   │   ├── Contracts/
│   │   │   ├── Dtos/                          # Data Transfer Objects
│   │   │   ├── Repositories/                  # Repository interfaces
│   │   │   └── Services/                      # Service interfaces
│   │   ├── UseCases/
│   │   │   ├── Shows/                         # Show queries and commands
│   │   │   │   ├── Queries/
│   │   │   │   │   ├── GetAllShowsQuery.cs
│   │   │   │   │   ├── GetShowByIdQuery.cs
│   │   │   │   │   ├── SearchShowsQuery.cs
│   │   │   │   │   └── GetUpcomingShowsQuery.cs
│   │   │   │   └── Commands/
│   │   │   │       ├── FilterShowsCommand.cs
│   │   │   │       └── SaveOrUpdateShowCommand.cs
│   │   │   ├── Users/                         # User registration and login
│   │   │   ├── Favorites/                     # Favorite management
│   │   │   └── Reviews/                       # Review submission
│   │   ├── Mapping/
│   │   │   └── MappingProfile.cs              # AutoMapper configuration
│   │   └── Validators/
│   │       ├── UserRegistrationValidator.cs   # Email, password rules
│   │       ├── UserLoginValidator.cs
│   │       └── CreateReviewValidator.cs       # Rating 1-5, comment length
│   │
│   ├── WroclawTheatreTickets.Infrastructure/  # Data Access & External Services
│   │   ├── Data/
│   │   │   └── TheatreDbContext.cs            # EF Core DbContext
│   │   ├── Repositories/                      # Repository implementations
│   │   │   ├── TheatreRepository.cs
│   │   │   ├── ShowRepository.cs              # Complex queries for filtering
│   │   │   ├── UserRepository.cs              # OAuth support
│   │   │   ├── FavoriteRepository.cs
│   │   │   ├── ReviewRepository.cs
│   │   │   ├── ViewHistoryRepository.cs
│   │   │   └── NotificationRepository.cs
│   │   ├── Services/                          # Service implementations
│   │   │   ├── AuthenticationService.cs       # JWT + BCrypt
│   │   │   ├── EmailService.cs                # SMTP email sender
│   │   │   ├── NotificationService.cs         # Push notifications stub
│   │   │   └── TheatreRepertoireSyncService.cs # Theatre data sync
│   │   └── Jobs/                              # Quartz.NET scheduled jobs
│   │       ├── SyncTheatreRepertoireJob.cs    # Daily at 2:00 AM
│   │       └── CleanupOldShowsJob.cs          # Weekly on Sunday 3:00 AM
│   │
│   └── WroclawTheatreTickets.Web/            # API Presentation Layer
│       ├── Program.cs                         # Application startup & DI
│       ├── ServiceCollectionExtensions.cs     # Service registration
│       ├── Endpoints.cs                       # Minimal API endpoints
│       ├── appsettings.json                   # Configuration (JWT, DB, Email)
│       └── appsettings.Development.json       # Dev overrides
│
├── tests/
│   ├── WroclawTheatreTickets.Domain.Tests/   # Domain entity tests
│   │   └── Entities/                          # 29 tests (User, Show, etc.)
│   ├── WroclawTheatreTickets.Application.Tests/ # Use case handler tests
│   │   └── UseCases/                          # 12 tests (Commands/Queries)
│   └── WroclawTheatreTickets.Infrastructure.Tests/ # Repository & service tests
│       ├── Repositories/                      # 15 tests (CRUD operations)
│       └── Services/                          # 10 tests (Auth, Email, etc.)
│
├── docs/
│   ├── BACKEND_SUMMARY.md                    # Complete architecture overview
│   ├── QUICK_START.md                        # Getting started guide
│   ├── ARCHITECTURE_DECISIONS.md             # Design rationale document
│   ├── TEST_COVERAGE.md                      # Test coverage report (66 tests)
│   ├── DEPENDENCIES.md                       # Dependency graph & versions
│   └── SESSION_LOGGING.md                    # AI coding session audit trail
│
├── WroclawTheatreTickets.slnx               # Visual Studio solution file
└── README.md                                 # This file
```

## ⚙️ Scheduled Jobs (Quartz.NET)

The application includes automated background jobs for maintenance and synchronization:

### 1. Theatre Repertoire Synchronization
**Schedule**: Daily at 2:00 AM  
**Job**: `SyncTheatreRepertoireJob`  
**Purpose**: Fetches latest theatre events from configured theatre APIs/websites

```csharp
// Cron: "0 0 2 ? * *" (Daily at 2 AM)
// Can be triggered manually with: dotnet run --force-sync
```

**Process**:
1. Calls `ITheatreRepertoireSyncService`
2. Fetches data from theatre websites
3. Deduplicates using `ExternalId`
4. Updates or creates Show entities
5. Logs success/failure counts

### 2. Cleanup Old Shows  
**Schedule**: Weekly on Sunday at 3:00 AM  
**Job**: `CleanupOldShowsJob`  
**Purpose**: Removes shows older than 2 years from database

```csharp
// Cron: "0 0 3 ? * SUN" (Sunday at 3 AM)
// Keeps database lean and performant
```

**Process**:
1. Calculates cutoff date (2 years ago)
2. Deletes shows with `StartDateTime < cutoff`
3. Logs deleted count
4. Related entities cascade deleted (reviews, favorites, etc.)

## 🔧 Configuration

### JWT Authentication Setup

⚠️ **CRITICAL**: Change the JWT secret in production!

Edit [`appsettings.json`](src/WroclawTheatreTickets.Web/appsettings.json):
```json
{
  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters-long!",
    "Issuer": "WroclawTheatreTickets",
    "Audience": "WroclawTheatreTicketsUsers"
  }
}
```

**Best Practices**:
- Minimum 32 characters
- Use environment variables in production: `%JWT_SECRET%`
- Rotate secrets periodically
- Never commit secrets to source control

### Database Configuration

**SQLite (Default)**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=theatre.db"
  }
}
```

**PostgreSQL (Production - future)**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=theatre;Username=user;Password=pass"
  }
}
```

### Email Service Configuration

Edit [`appsettings.json`](src/WroclawTheatreTickets.Web/appsettings.json):
```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "From": "noreply@wroclawtheatretickets.pl",
    "Password": "your-app-specific-password"
  }
}
```

**Gmail Setup**:
1. Enable 2FA on your Google account
2. Generate App Password at [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)
3. Use app password (not your regular password)

### OAuth Configuration (Optional)

For Google/Facebook authentication, add to `appsettings.json`:
```json
{
  "OAuth": {
    "Google": {
      "ClientId": "your-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-client-secret"
    },
    "Facebook": {
      "AppId": "your-app-id",
      "AppSecret": "your-app-secret"
    }
  }
}
```

### CORS Configuration

Development (allow all):
```csharp
// Already configured in ServiceCollectionExtensions.cs
services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

Production (restrict to frontend domain):
```csharp
builder.AllowOrigins("https://yourfrontend.com")
       .AllowCredentials();
```

## 🧪 Testing

**Current Coverage**: 66 tests across all layers (100% passing)

### Run All Tests
```powershell
dotnet test
```

**Expected Output**:
```
Passed! - Failed: 0, Passed: 66, Skipped: 0, Total: 66, Duration: ~1.25s
```

### Test Breakdown

**Domain Layer** (29 tests):
- User entity creation and validation
- Show entity lifecycle and business rules
- User interactions (favorites, reviews, view history)
- Theatre entity management

**Application Layer** (12 tests):
- CQRS command handlers (RegisterUser, FilterShows)
- Query handlers with mocked repositories
- FluentValidation integration
- DTOmapping scenarios

**Infrastructure Layer** (25 tests):
- Repository CRUD operations
- Authentication service (JWT, BCrypt)
- Email service stub
- Database context configuration

### Run Specific Test Project
```powershell
dotnet test tests/WroclawTheatreTickets.Domain.Tests
dotnet test tests/WroclawTheatreTickets.Application.Tests
dotnet test tests/WroclawTheatreTickets.Infrastructure.Tests
```

### Generate Coverage Report (optional)
```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

See [TEST_COVERAGE.md](./docs/TEST_COVERAGE.md) for detailed coverage report.

## 📈 Performance Optimizations

### Application Level
- ✅ **Async/Await**: All I/O operations (database, HTTP) use async patterns
- ✅ **Database Indexes**: Strategic indexes on high-traffic columns:
  - `Show.StartDateTime`, `Show.IsActive`, `Show.ExternalId`
  - `User.Email`, `UserFavorite.UserId+ShowId`
  - `Review.ShowId+IsApproved`
- ✅ **Eager Loading**: Optimized queries with `.Include()` to prevent N+1 queries
- ✅ **DTO Projection**: AutoMapper profiles minimize over-fetching
- ✅ **Structured Logging**: Async Serilog with file buffering doesn't block requests
- ✅ **Connection Pooling**: EF Core manages database connection pool

### Query Optimization Examples
```csharp
// Optimized: Single query with Include
var shows = await _context.Shows
    .Include(s => s.Theatre)
    .Include(s => s.Reviews.Where(r => r.IsApproved))
    .Where(s => s.IsActive)
    .ToListAsync();
```

### Future Performance Enhancements
- [ ] **Redis Caching**: Cache frequently accessed shows (trending, upcoming)
- [ ] **Response Compression**: Gzip/Brotli for large JSON responses
- [ ] **Database**: Migrate to PostgreSQL for production scale
- [ ] **CDN**: Static content (images, posters) on CDN
- [ ] **Rate Limiting**: Prevent API abuse with rate limits

## 🔒 Security Features

### ✅ Implemented
- **Password Security**: BCrypt hashing with salt (cost factor: 11)
- **Authentication**: JWT Bearer tokens with expiration (default: 7 days)
- **Authorization**: Role-based access control (User, Moderator, Admin)
- **HTTPS Enforcement**: `UseHttpsRedirection()` in production
- **CORS Policy**: Configurable cross-origin rules
- **Input Validation**: FluentValidation on all user inputs
- **SQL Injection Protection**: EF Core parameterized queries
- **Sensitive Data**: Passwords never logged or returned in responses
- **Token Security**: Asymmetric JWT signing (HS256)

### Password Requirements
Enforced in `UserRegistrationValidator.cs`:
- Minimum 8 characters
- At least one uppercase letter (recommended)
- At least one lowercase letter (recommended)
- At least one digit (recommended)
- At least one special character (recommended)

### ⚠️ Production Security Checklist
- [ ] Change JWT secret to cryptographically secure random value
- [ ] Enable rate limiting (e.g., AspNetCoreRateLimit)
- [ ] Add request size limits
- [ ] Implement API key authentication for background jobs
- [ ] Set up Content Security Policy (CSP) headers
- [ ] Enable SQL query logging only in development
- [ ] Use Azure Key Vault or AWS Secrets Manager for secrets
- [ ] Configure firewall rules for database access
- [ ] Enable HTTPS only (disable HTTP)
- [ ] Implement refresh tokens for long-lived sessions

## 📝 Logging

### Serilog Configuration

Structured logging with multiple sinks:
- **Console**: Real-time output during development
- **File**: Rolling logs in `logs/` directory (daily rotation)

**Log Location**: `logs/app{Date}.txt`
```
logs/app20260209.txt
logs/app20260210.txt
```

### Log Levels
```csharp
Log.Information("User {UserId} logged in", userId);
Log.Warning("Failed login attempt for {Email}", email);
Log.Error(exception, "Database error in {Operation}", "GetShows");
```

### Example Log Output
```json
{
  "@t": "2026-02-09T14:30:12.123Z",
  "@l": "Information",
  "@mt": "User {UserId} logged in",
  "UserId": "guid-here",
  "SourceContext": "WroclawTheatreTickets.Web.Endpoints"
}
```

### Future Integrations
- [ ] **Seq**: Real-time log search and analysis
- [ ] **Application Insights**: Azure monitoring
- [ ] **ELK Stack**: Elasticsearch, Logstash, Kibana
- [ ] **Sentry**: Error tracking and alerting

## 🐛 Troubleshooting

### Common Issues

**1. Port Already in Use**

Symptom: `Address already in use: 'https://localhost:5001'`

Solution A - Kill process on port:
```powershell
netstat -ano | findstr :5001
taskkill /PID <process-id> /F
```

Solution B - Change port in [launchSettings.json](src/WroclawTheatreTickets.Web/Properties/launchSettings.json):
```json
"applicationUrl": "https://localhost:5002;http://localhost:5003"
```

**2. Database Errors**

Symptom: `SQLite Error 1: 'no such table: Shows'`

Solution - Reset database:
```powershell
cd src\WroclawTheatreTickets.Web
Remove-Item theatre.db
dotnet run  # Database auto-created on startup
```

**3. Build Failures**

Symptom: `error CS0246: The type or namespace name 'X' could not be found`

Solution - Clean and restore:
```powershell
dotnet clean
Remove-Item -Recurse -Force **/bin, **/obj
dotnet restore
dotnet build WroclawTheatreTickets.slnx
```

**4. Authentication Failures**

Symptom: `401 Unauthorized` even with valid token

Check:
- JWT secret matches in `appsettings.json`
- Token not expired (check `exp` claim)
- `Authorization: Bearer <token>` header format correct
- Token contains required claims (`sub`, `email`, `role`)

**5. Test Failures**

Symptom: Tests fail intermittently

Solution:
```powershell
# Clean test artifacts
Remove-Item -Recurse tests/**/bin, tests/**/obj
dotnet test --no-build --verbosity detailed
```

**6. Quartz.NET Job Not Running**

Symptom: Scheduled jobs don't execute

Check:
- Logs in `logs/app{Date}.txt` for job registration
- Manually trigger: `dotnet run --force-sync`
- Ensure `AddQuartzHostedService()` is registered

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Use meaningful commit messages
3. Add tests for new features
4. Update documentation

## � Documentation

Comprehensive documentation available in [`docs/`](docs/) folder:

| Document | Purpose |
|----------|--------|
| [BACKEND_SUMMARY.md](./docs/BACKEND_SUMMARY.md) | Complete architecture overview, features, and database schema |
| [QUICK_START.md](./docs/QUICK_START.md) | API usage examples with curl commands |
| [ARCHITECTURE_DECISIONS.md](./docs/ARCHITECTURE_DECISIONS.md) | Design rationale and technology choices |
| [TEST_COVERAGE.md](./docs/TEST_COVERAGE.md) | Detailed test coverage report (66 tests) |
| [DEPENDENCIES.md](./docs/DEPENDENCIES.md) | Dependency graph and version management |
| [SESSION_LOGGING.md](./docs/SESSION_LOGGING.md) | AI coding session audit trail setup |

## 🤝 Contributing

### Guidelines
1. **Follow Clean Architecture**: Respect layer boundaries (Domain → Application → Infrastructure → Web)
2. **Write Tests**: Add unit tests for new features (target: >80% coverage)
3. **Use Conventional Commits**: `feat:`, `fix:`, `docs:`, `test:`, `refactor:`
4. **Update Documentation**: Keep docs in sync with code changes
5. **Code Style**: Follow .NET coding conventions and use `.editorconfig`

### Workflow
```bash
git checkout -b feature/your-feature-name
# Make changes
dotnet build
dotnet test
git commit -m "feat: add your feature description"
git push origin feature/your-feature-name
# Create Pull Request
```

## 📄 License

This project is licensed under the MIT License - see LICENSE file for details.

## 👥 Authors & Contributors

- **Project Lead**: Maksim Hatoutsau
- **Architecture**: Clean Architecture pattern
- **Framework**: .NET 10

### Special Thanks
- ASP.NET Core team for excellent framework
- Clean Architecture community for best practices
- Wrocław theatre community for inspiration

## 📞 Support & Contact

### Issues & Feature Requests
1. **Check existing documentation** in [`docs/`](docs/) folder
2. **Search existing issues** on GitHub
3. **Create new issue** with template:
   - **Bug Report**: Steps to reproduce, expected vs actual behavior
   - **Feature Request**: Use case, proposed solution, alternatives

### Questions
- **Technical**: Review [ARCHITECTURE_DECISIONS.md](./docs/ARCHITECTURE_DECISIONS.md)
- **API Usage**: See [QUICK_START.md](./docs/QUICK_START.md)
- **Getting Started**: Follow this README from the top

## 🎓 Learning Resources

### Architecture & Patterns
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern - Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- [Domain-Driven Design Fundamentals](https://www.pluralsight.com/courses/domain-driven-design-fundamentals)
- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)

### .NET & EF Core
- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core 10](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [Minimal APIs Guide](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)

### Libraries Used
- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [AutoMapper Documentation](https://docs.automapper.org/)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Getting-Started)
- [Quartz.NET Tutorial](https://www.quartz-scheduler.net/documentation/)

### Testing
- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

## 📊 Project Status

**Current Version**: 1.0.0  
**Status**: ✅ **PRODUCTION READY** (requires configuration)

### Implementation Progress
- ✅ Core Architecture (100%)
- ✅ Domain Layer (100%)
- ✅ Application Layer (100%)
- ✅ Infrastructure Layer (100%)
- ✅ Web API Layer (100%)
- ✅ Authentication & Authorization (100%)
- ✅ Scheduled Jobs (100%)
- ✅ Unit Tests (66 tests, 100% passing)
- 🚧 Theatre Parsers (Framework ready, site-specific implementations needed)
- 🚧 Email Service (SMTP configured, credentials needed)
- 🚧 Push Notifications (Framework ready, provider integration needed)

### Next Steps
1. Configure theatre website parsers for Wrocław venues
2. Set up production database (PostgreSQL recommended)
3. Deploy to cloud (Azure, AWS, or DigitalOcean)
4. Configure email credentials for notifications
5. Integrate push notification provider (Firebase/OneSignal)
6. Set up frontend application

---

**Built with ❤️ for Wrocław Theatre Lovers**

Last Updated: February 9, 2026  
Maintained by: [Maksim Hatoutsau](https://github.com/mhatoutsau)
