---
date: 2026-02-09T13:11:00Z
role: assistant
conversation_id: session-wroclaw-theatre-cleanup
context: Comprehensive DI registration audit results
redacted: false
---

## DI Registration Audit Complete ✅

Analyzed all dependency injection registrations across the solution. Here's the complete status:

### ✅ CORRECTLY REGISTERED

**Database Context:**
- `TheatreDbContext` → Scoped (EF Core with SQLite)

**MediatR:**
- ✅ Fixed: Now scanning Application assembly for all handlers
- Discovers: SaveOrUpdateShowCommandHandler, FilterShowsCommandHandler, and all query handlers

**Repositories (Application.Contracts.Repositories → Infrastructure.Repositories):**
- `ITheatreRepository` → `TheatreRepository` (Scoped)
- `IShowRepository` → `ShowRepository` (Scoped)
- `IUserRepository` → `UserRepository` (Scoped)
- `IFavoriteRepository` → `FavoriteRepository` (Scoped)
- `IReviewRepository` → `ReviewRepository` (Scoped)
- `IViewHistoryRepository` → `ViewHistoryRepository` (Scoped)
- `INotificationRepository` → `NotificationRepository` (Scoped)

**Services (Application.Contracts.Services → Infrastructure.Services):**
- `IAuthenticationService` → `AuthenticationService` (Scoped)
- `IEmailService` → `EmailService` (Scoped)
- `INotificationService` → `NotificationService` (Scoped)
- `ITheatreRepertoireSyncService` → `TheatreRepertoireSyncService` (Scoped)

**AutoMapper:**
- `MappingProfile` registered and scanned

**Background Jobs (Quartz.NET in Program.cs):**
- `SyncTheatreRepertoireJob` → Registered with daily 2 AM trigger
- `CleanupOldShowsJob` → Registered with weekly Sunday 3 AM trigger
- `ISchedulerFactory` → Provided by Quartz
- `IHttpClientFactory` → Registered for API calls

**Authentication & Authorization:**
- JWT Bearer authentication configured
- Policies: "AdminOnly", "ModeratorOrAdmin"

**Other:**
- CORS policy "AllowAll" configured
- Swagger/OpenAPI configured
- HTTP Client Factory registered

### ⚠️ ISSUES FOUND

**1. FluentValidation Scanning Wrong Assembly**
```csharp
// Current (INCORRECT - scans Web assembly)
services.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensions));

// Should be (Application assembly where validators live)
services.AddValidatorsFromAssemblyContaining<SaveOrUpdateShowCommand>();
```

**2. ICacheService Interface Defined But Not Implemented**
- Interface exists in `IServices.cs`
- No implementation found in Infrastructure
- Not registered in DI
- **Action:** Either implement CacheService or remove interface if not needed

### 📋 SUMMARY

**Total Registrations:** 19
**Correctly Registered:** 17
**Issues:** 2

**Repository Coverage:** 7/7 interfaces implemented and registered ✅
**Service Coverage:** 4/5 interfaces implemented (ICacheService missing) ⚠️
**MediatR Handler Discovery:** Fixed to scan Application assembly ✅
**Quartz Jobs:** Both jobs properly registered ✅
**Validator Discovery:** Needs fix to scan Application assembly ⚠️

### 🔧 RECOMMENDED FIXES

1. **Fix FluentValidation assembly scan**
2. **Decide on ICacheService:** Either implement it (for Redis/memory caching) or remove the interface if not planning to use it
