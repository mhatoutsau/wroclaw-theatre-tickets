# Quick Start Guide - Wrocław Theatre Tickets Backend

## Prerequisites
- .NET 10 SDK installed
- SQLite (included with .NET)
- A code editor (VS Code, Visual Studio, etc.)

## Running the Application

### 1. Navigate to Web Project
```powershell
cd src/WroclawTheatreTickets.Web
```

### 2. Start the Application
```powershell
dotnet run
```

### 3. Access the API
- **API Base URL**: `https://localhost:5001/api`
- **Swagger UI**: `https://localhost:5001/swagger` (when in Development mode)
- **Health Check**: `GET https://localhost:5001/health`

## Sample API Calls

### 1. Register a New User
```bash
curl -X POST "https://localhost:5001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePass123"
  }'
```

**Response:**
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "accessToken": "eyJhbGc...",
  "expiresAt": "2025-02-09T15:30:00Z"
}
```

### 2. Login
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePass123"
  }'
```

### 3. Get All Shows
```bash
curl -X GET "https://localhost:5001/api/shows" \
  -H "Accept: application/json"
```

### 4. Search Shows
```bash
curl -X GET "https://localhost:5001/api/shows/search?keyword=opera" \
  -H "Accept: application/json"
```

### 5. Get Upcoming Shows
```bash
curl -X GET "https://localhost:5001/api/shows/upcoming?days=30" \
  -H "Accept: application/json"
```

### 6. Filter Shows (Advanced)
```bash
curl -X POST "https://localhost:5001/api/shows/filter" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Opera",
    "dateFrom": "2025-02-09T00:00:00Z",
    "dateTo": "2025-03-09T23:59:59Z",
    "priceFrom": 0,
    "priceTo": 200,
    "language": "Polish"
  }'
```

### 7. Add Show to Favorites (Authenticated)
```bash
curl -X POST "https://localhost:5001/api/favorites/550e8400-e29b-41d4-a716-446655440001" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 8. Get User Favorites (Authenticated)
```bash
curl -X GET "https://localhost:5001/api/favorites" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 9. Create a Review (Authenticated)
```bash
curl -X POST "https://localhost:5001/api/reviews" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -d '{
    "showId": "550e8400-e29b-41d4-a716-446655440001",
    "rating": 5,
    "comment": "Excellent performance!"
  }'
```

## Database Management

### Initialize Database
The database is automatically created and migrated on first run. To reset:

```powershell
# Delete the database file
Remove-Item theatre.db

# Run the application to recreate
dotnet run
```

### View Database (with SQLite CLI)
```powershell
# Install SQLite tools (if needed)
choco install sqlite

# Open database
sqlite3 theatre.db

# List tables
.tables

# Query shows
SELECT Title, StartDateTime FROM Shows LIMIT 10;
```

## Configuration

### JWT Secret (IMPORTANT - Change in Production!)
Edit `appsettings.json`:
```json
"Jwt": {
  "Secret": "your-super-secret-key-at-least-32-characters-long",
  "Issuer": "WroclawTheatreTickets",
  "Audience": "WroclawTheatreTicketsUsers"
}
```

### Email Configuration
```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "From": "noreply@example.com",
  "Password": "your-app-password"
}
```

### Database Connection
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=theatre.db"
}
```

## Common Tasks

### Add a Theater (Seed Data)
```csharp
// In a separate seeding project or migration
var theatre = Theatre.Create(
    "Opera Wrocławska",
    "ul. Świdnicka 35",
    "+48 71 370 85 00",
    "opera@teatr.wroclaw.pl",
    "https://www.operapolska.pl"
);
await theatreRepository.AddAsync(theatre);
```

### Schedule Daily Parsing Job
Use Quartz.NET (already in project):

```csharp
// In ServiceCollectionExtensions
services.AddQuartz(q =>
{
    var jobKey = new JobKey("ParseTheatresJob");
    q.AddJob<ParseTheatresJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ParseTheatresTrigger")
        .WithCronSchedule("0 0 2 * * ?")  // 2 AM daily
    );
});
services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
```

### Enable Swagger in Production
Edit `appsettings.json`:
```json
"Swagger": {
  "Enabled": true  // Add if needed for production
}
```

## Troubleshooting

### Port Already in Use
```powershell
# Change port in appsettings.json
"Kestrel": {
  "Endpoints": {
    "Https": {
      "Url": "https://localhost:5002"
    }
  }
}
```

### Database Locked
```powershell
# Ensure no other process is using the database
# Delete theatre.db and restart
```

### JWT Token Validation Fails
- Check that the secret key matches between issuing and validation
- Verify token hasn't expired
- Ensure "Bearer" prefix in Authorization header

### Build Fails
```powershell
dotnet clean
dotnet restore
dotnet build
```

## Performance Tips

1. **Caching**: Add Redis for frequently accessed data
2. **Pagination**: Implement pagination for large result sets
3. **Indexing**: Database indexes already configured for common queries
4. **Async**: All I/O operations are async for scalability

## Security Checklist

- [ ] Change JWT secret in production
- [ ] Configure CORS for specific domains
- [ ] Enable HTTPS only in production
- [ ] Setup rate limiting
- [ ] Configure email service with real SMTP
- [ ] Setup database backups
- [ ] Enable audit logging

## Support & Documentation

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
