---
date: 2026-02-09T13:13:00Z
role: assistant
conversation_id: session-wroclaw-theatre-cleanup
context: Fixed SQLite database initialization issue
redacted: false
---

## SQLite Database Initialization Fixed ✅

**Problem:** Database tables didn't exist - getting "SQLite Error 1: 'no such table: Shows'"

**Root Cause:** 
- No EF Core migrations were created
- Program.cs was calling `Database.Migrate()` which requires migrations to exist
- Migration generation was failing due to missing `Microsoft.EntityFrameworkCore.Design` package

**Solution Implemented:**

### 1. Added Required Package
```bash
dotnet add src\WroclawTheatreTickets.Web package Microsoft.EntityFrameworkCore.Design --version 10.0.0
```

### 2. Changed Database Initialization Strategy
Updated [Program.cs](src/WroclawTheatreTickets.Web/Program.cs):

```csharp
// Before (requires migrations)
dbContext.Database.Migrate();

// After (creates schema automatically)
dbContext.Database.EnsureCreated();
```

**What `EnsureCreated()` Does:**
- Checks if database exists
- If not, creates it with all tables based on DbContext configuration
- Suitable for development and testing
- Does NOT use migrations (creates schema directly from model)

### Database Schema Created

The database will now include these tables:
- **Theatres** - Theatre venues
- **Shows** - Show/event listings with external ID support
- **Users** - User accounts with OAuth support
- **UserFavorites** - Favorited shows (many-to-many)
- **ViewHistories** - User view tracking
- **Reviews** - User reviews with ratings
- **Notifications** - User notifications

**Next Steps:**
- Run the application - database will be automatically created on first startup
- For production, consider creating proper migrations with:
  ```bash
  dotnet ef migrations add InitialCreate --project src\WroclawTheatreTickets.Infrastructure --startup-project src\WroclawTheatreTickets.Web
  ```

**Build Status:** ✅ Ready to run - database will be created automatically on startup
