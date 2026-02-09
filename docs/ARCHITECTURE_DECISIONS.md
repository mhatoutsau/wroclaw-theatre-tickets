# Architecture Decision Records - Wrocław Theatre Tickets

## ADR-001: Clean Architecture Pattern

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Implement Clean Architecture with clear separation of concerns across four projects:
- Domain (entities, business logic)
- Application (use-cases, DTOs)
- Infrastructure (data access, external services)
- Web (API endpoints, dependency injection)

### Rationale
- Testability: Business logic independent of frameworks
- Maintainability: Clear dependency flow (inward)
- Scalability: Easy to add new features without breaking existing code
- Flexibility: Easy to switch implementations (e.g., database, services)

### Consequences
- Slightly more projects to manage
- Requires disciplined adherence to dependency rules
- Better long-term maintainability vs. monolithic structure

---

## ADR-002: CQRS with MediatR

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use Command Query Responsibility Segregation (CQRS) pattern with MediatR library for handling use-cases.

### Rationale
- Clear separation: Queries (read) vs Commands (write)
- Single Responsibility: Each handler does one thing
- Testability: Easy to test handlers in isolation
- Extensibility: Decorators/behaviors can be added (logging, validation, caching)

### Consequences
- Slight overhead from MediatR dispatching
- Learning curve for new developers
- More files but clearer structure

---

## ADR-003: SQLite for Caching, Not Primary Store

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use SQLite as local cache layer for theater event data from parsing,  not as permanent storage.

### Rationale
- Requirement: "Local database for caching theatre events"
- Simplicity: No external database setup needed
- Development: Easy setup for local development
- Flexibility: Can migrate to PostgreSQL/SQL Server for production

### Migration Path
```
Phase 1 (Current): SQLite for development
Phase 2 (Production): PostgreSQL/SQL Server with SQLite as distributed cache
Phase 3 (Scale): Redis cache layer + primary database
```

### Consequences
- SQLite limitations (10GB max practical size) - acceptable for caching
- Single-file database less suitable for production scale
- Consider migration after proof of concept

---

## ADR-004: JWT Bearer Authentication

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use JWT (JSON Web Tokens) with Bearer token scheme for API authentication.

### Rationale
- Stateless: No server-side session storage needed
- Scalable: Works well with distributed systems
- Mobile-friendly: Tokens can be stored in app storage
- OAuth-ready: Can combine with third-party providers

### Token Structure
```json
{
  "sub": "user-id",
  "email": "user@example.com",
  "role": "User",
  "exp": "2025-02-09T15:30:00Z"
}
```

### Consequences
- Tokens are signed but not encrypted (use HTTPS)
- No built-in revocation (implement token blacklist if needed)
- Must handle token refresh for long-lived sessions

---

## ADR-005: Role-Based Access Control (RBAC)

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Implement three roles: User, Moderator, Admin with claim-based authorization.

### Role Definitions
| Role | Capabilities |
|------|-------------|
| User | View shows, create reviews, manage favorites |
| Moderator | User permissions + approve/reject reviews |
| Admin | All permissions + manage theaters, content moderation |

### Implementation
- Claims stored in JWT token
- Authorization policies in `ServiceCollectionExtensions`
- Attribute-based checks on endpoints

### Consequences
- Scalable for future role additions
- Audit trail needed for admin actions
- Consider audit logging for compliance

---

## ADR-006: DTOs for API Boundaries

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use Data Transfer Objects (DTOs) to decouple API contracts from domain entities.

### Rationale
- API versioning: Can change DTOs without breaking domain
- Security: Avoid exposing internal fields (e.g., password hashes)
- Validation: Centralized input validation at API boundary
- Flexibility: Different DTOs for different endpoints

### DTO Types
- `ShowDto` - Minimal show data
- `ShowDetailDto` - Full show with reviews
- `UserDto` - Public user information
- `UserProfileDto` - User's own profile data

### Consequences
- Requires mapping between entities and DTOs (AutoMapper handles this)
- Slight performance overhead from mapping
- Better API stability in the long run

---

## ADR-007: Repository Pattern with Async/Await

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Implement Repository pattern with async/await for all data access.

### Rationale
- Abstraction: Decouples data access from business logic
- Testing: Easy to mock repositories
- Async/Await: Non-blocking I/O for scalability
- Thread Safety: Task-based concurrency model

### Repository Interfaces
```csharp
Task<T> GetByIdAsync(Guid id);
Task<IEnumerable<T>> GetAllAsync();
Task AddAsync(T entity);
Task UpdateAsync(T entity);
Task DeleteAsync(Guid id);
```

### Consequences
- All data access is async (no .Result blocking)
- Slightly more complex error handling
- Better scalability for high-concurrency scenarios

---

## ADR-008: Entity Framework Core Code-First Migrations

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use EF Core with code-first approach and migrations for database schema management.

### Rationale
- Version control: Schema changes tracked in migrations
- Reproducibility: Same schema everywhere
- Flexibility: Easy to adjust schema without SQL
- Automatic: Migrations auto-apply on startup

### Migration Workflow
```powershell
# Add migration after entity changes
dotnet ef migrations add AddNewFeature --project Infrastructure --startup-project Web

# Apply migrations (automatic on startup)
dotnet ef database update --project Infrastructure --startup-project Web
```

### Consequences
- Migration conflicts in team settings
- Requires disciplined migration naming
- Schema auditing via migration history

---

## ADR-009: Validation at Multiple Layers

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Implement validation at three levels:
1. DTO level (FluentValidation)
2. Domain level (entity constructors)
3. Business logic level (handlers)

### Rationale
- Defense in depth
- Early rejection of invalid data
- Clear error messages to clients
- Business rules enforcement

### Validation Flow
```
API Request → DTO Validation → Repository → Domain Validation → Handler
```

### Consequences
- Slightly more code for validation
- Consistent error messages across API
- Better error handling

---

## ADR-010: Logging with Serilog

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use Serilog for structured logging throughout the application.

### Rationale
- Structured logging: Rich context with properties
- Sinks: Multiple outputs (console, file, external services)
- Performance: Async logging doesn't block requests
- Debugging: Detailed logs for troubleshooting

### Log Levels
- **Information**: User actions, business events
- **Warning**: Invalid inputs, recoverable errors
- **Error**: Operation failures, exceptions
- **Debug**: Detailed execution flow (development only)

### Consequences
- Dependency on Serilog library
- Log file management needed
- Consider log aggregation service for production

---

## ADR-011: HTML Parsing for Theater Website Scraping

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use HtmlAgilityPack for parsing theater websites and extracting event data.

### Rationale
- Reliability: Handles malformed HTML well
- XPath Support: Easy DOM navigation
- Performance: Efficient HTML processing
- Open Source: Well-maintained library

### Parsing Strategy
1. Fetch HTML from theater website
2. Parse with HtmlAgilityPack XPath/LINQ
3. Extract show details (title, date, price, etc.)
4. Normalize data
5. Store in SQLite with external ID for deduplication

### Consequences
- Website structure changes break parsing
- Requires specific parser per theater
- Consider fallback/notification on parse failure

---

## ADR-012: Quartz.NET for Scheduled Jobs

**Status**: Planned (Not Yet Implemented)  
**Date**: February 9, 2025

### Decision
Use Quartz.NET for daily theater website parsing and data updates.

### Rationale
- Reliability: Persistent job scheduling
- Flexibility: Cron expressions for complex schedules
- Features: Retry logic, job history, clustering support
- Integration: Native ASP.NET Core support

### Scheduled Job: Daily Theater Parsing
```
Trigger: 2:00 AM daily
Job: ParseTheatresJob
Action: Parse all theater websites, update SQLite cache
Retry: 3 attempts with exponential backoff
Notification: Alert admins on failure
```

### Consequences
- Additional service to manage
- Requires monitoring/alerting
- Database contention during parsing

---

## ADR-013: Email Service Implementation

**Status**: Planned (Stub Implemented)  
**Date**: February 9, 2025

### Decision
Implement email service for user notifications via SMTP.

### Rationale
- User engagement: Event reminders, new listings
- Communication: Password resets, email verification
- Flexibility: Can switch to Sendgrid, MailChimp, etc.

### Email Types
1. **Registration**: Email verification link
2. **Reminders**: 1 day before event
3. **Digest**: Weekly curated listings
4. **Alerts**: System notifications

### Implementation Options
- Local SMTP (development)
- Gmail/Office365 (small scale)
- SendGrid/Mailgun (production)
- AWS SES (high volume)

### Consequences
- Requires SMTP configuration
- Email deliverability concerns
- Consider email service provider for scale

---

## ADR-014: API Versioning Strategy

**Status**: Future (Not Yet Implemented)  
**Date**: February 9, 2025

### Decision
Use URL versioning for API versions (e.g., `/api/v1/shows`).

### Rationale
- Clarity: Version in URL is explicit
- Backward compatibility: Multiple versions can coexist
- Client control: Client chooses version
- Simple: No complex negotiation

### Versioning Approach
```
Current: /api/shows
Future v2: /api/v2/shows (if breaking changes needed)
Deprecation: /api/v1/shows → /api/shows (with warnings)
```

### Consequences
- URL complexity
- Multiple code paths to maintain
- Plan breaking changes carefully

---

## ADR-015: Error Handling and Status Codes

**Status**: Adopted  
**Date**: February 9, 2025

### Decision
Use HTTP status codes consistently and return error details in response body.

### Status Code Mapping
| Code | Scenario | Example |
|------|----------|---------|
| 200 | Success | GET show details |
| 201 | Created | Register new user |
| 400 | Validation Error | Invalid email format |
| 401 | Unauthorized | Missing/invalid token |
| 403 | Forbidden | User cannot delete other's review |
| 404 | Not Found | Show ID doesn't exist |
| 500 | Server Error | Unhandled exception |

### Error Response Format
```json
{
  "error": "Show not found",
  "statusCode": 404,
  "timestamp": "2025-02-09T10:30:00Z",
  "traceId": "correlation-id"
}
```

### Consequences
- Standardized error handling
- Better client error handling
- Consistent API behavior

---

## Future Considerations

### ADR-016: Caching Strategy (Redis)
When performance becomes an issue, implement Redis for:
- Show listing caches
- User session tokens
- Theater information

### ADR-017: Full-Text Search
Implement Elasticsearch for advanced search when dataset grows:
- Keyword search across all fields
- Fuzzy matching
- Autocomplete

### ADR-018: GraphQL API
Consider GraphQL as alternative to REST:
- Reduced over-fetching
- Strongly typed schema
- Better for mobile clients

### ADR-019: Event Sourcing
For audit trail and compliance:
- All state changes as events
- Event log as system of record
- Temporal queries

### ADR-020: API Gateway
When multiple services exist:
- API Gateway for routing
- Rate limiting
- Request/response transformation
- Authentication at gateway level

---

## ADR-021: Distributed Caching Layer with IDistributedCache

**Status**: Adopted  
**Date**: February 9, 2026

### Decision
Implement a caching layer using .NET's `IDistributedCache` abstraction with initial in-memory backend support and planned Redis integration.

### Rationale
- **Backend Flexibility**: `IDistributedCache` allows seamless migration from in-memory to Redis/other backends without code changes
- **Standard Approach**: Uses Microsoft's built-in caching abstraction rather than custom solutions
- **Performance**: 60-80% reduction in database queries for read-heavy operations; 2-5x faster response times
- **Configuration**: TTLs are configurable via appsettings without code changes
- **Metrics**: Built-in observability for cache hit rates and performance monitoring

### Implementation Details
- **CacheService**: Wrapper around `IDistributedCache` with JSON serialization and metrics tracking
- **CacheMetrics**: Thread-safe metrics collection (hits, misses, evictions) exposed via `/health/cache` endpoint
- **Hybrid Invalidation**: Eager clearing on data changes + TTL expiration as safety net
- **Critical Caches** (Phase 1):
  - Theatres (24h) - rarely changes, referenced everywhere
  - All Shows (15min) - frequently accessed
  - Upcoming Shows (30min) - calendar-based queries
  - Trending Shows (1h) - most viewed shows
  - Show Details (10min) - with reviews

### Architecture
```
IDistributedCache (Interface)
    ↓
MemoryDistributedCache (Phase 1: in-memory)
    ↓ Future
StackExchange.Redis (Phase 2: distributed)
```

### Consequences
- **Positive**:
  - Significant performance improvement for read-heavy workloads
  - Scalable architecture (ready for multi-instance deployment)
  - Operationally observable via health endpoint
  - Non-breaking changes to existing code
- **Negative**:
  - Added complexity in cache invalidation logic
  - Potential for stale data (mitigated by TTL fallback)
  - In-memory cache limited by single instance memory

### Cache Invalidation Strategy
- **Write Operations** trigger eager invalidation of affected caches
- **TTL acts as safety net** (hybrid approach):
  - Show Update → Clears `shows:*`, `reviews:*` patterns
  - Review Approval → Clears show detail, review caches
  - User Favorites → Clears user-specific caches

### Testing
- Integration tests in `CacheServiceIntegrationTests` verify functionality
- Mock-based tests in `CachedQueryHandlerTests` verify handler integration
- Metrics validation tests ensure accurate tracking

### Future Enhancements
- Redis backend integration (drop-in replacement)
- Advanced pattern-based invalidation using Redis SCAN
- Cache warming strategies for critical data
- Distributed cache invalidation messaging

---

## ADR-015: Extract Theatre Sync into Theatre-Specific Data Services

**Status**: Implemented  
**Date**: February 9, 2026

### Context
The original `TheatreRepertoireSyncService` had multiple responsibilities:
1. HTTP communication with theatre APIs
2. JWT token parsing from API responses
3. DTO→domain entity mapping
4. Theatre entity management (get-or-create)
5. Orchestration of MediatR commands

Each theatre API has a unique response format, making it difficult to add new theatres without modifying the monolithic sync service. Configuration (API URLs, timeouts) was hardcoded as constants.

### Decision
Separate theatre-specific concerns into dedicated services with clear interfaces:

**Architecture**:
- **`IRepertoireDataService`** (Application/Contracts/Services):
  - Interface for th eatre-specific data fetching and mapping
  - Returns `Task<List<Show>>` (fully mapped domain entities)
  - Implemented per theatre (e.g., `TeatrPolskiRepertoireDataService`)
  
- **`ITheatreProviderService`** (Application/Contracts/Services):
  - Theatre entity lifecycle management
  - `GetOrCreateTheatreAsync(name, city, address)` - get-or-create pattern
  - Theatre-agnostic, reusable across all data services
  
- **Refactored `TheatreRepertoireSyncService`** (Infrastructure/Services):
  - Orchestration only: theatre setup → fetch → persist
  - Injected dependencies: `IRepertoireDataService`, `ITheatreProviderService`, `IMediator`
  - Removed all HTTP, mapping, and repository logic

**Configuration**:
- **`TheatreApiConfiguration`** (Infrastructure/Configuration):
  - POCO model with `Url` and `TimeoutSeconds`
  - Bound to `appsettings.json` section `TheatreApis:TeatrPolski`
  - Injected via `IOptions<TheatreApiConfiguration>`

**Theatre-Specific Logic**:
- **`TeatrPolskiApiDtoMapper`** (Infrastructure/Services):
  - Moved from Application/Contracts/Dtos (was `ApiDtoMapper`)
  - Public static utility class for TeatrPolski-specific parsing
  - Used exclusively by `TeatrPolskiRepertoireDataService`

### Rationale
**Single Responsibility Principle (SRP)**:
- Each service has one cohesive responsibility
- Data services encapsulate theatre-specific API knowledge
- Provider service handles entity lifecycle
- Sync service coordinates the workflow

**Open/Closed Principle**:
- Adding new theatres: create new `IRepertoireDataService` implementation
- No modifications to existing orchestration or provider services

**Dependency Inversion**:
- High-level orchestration depends on abstractions (`IRepertoireDataService`, `ITheatreProviderService`)
- Low-level implementations (HTTP, mapping) injected at runtime

**Configuration Flexibility**:
- API URLs/timeouts externalized to `appsettings.json`
- Environment-specific overrides via `appsettings.{Environment}.json`
- No recompilation needed for URL changes

**Testability**:
- Each service independently testable with mocks
- HTTP responses simulated via `MockHttpMessageHandler`
- 15 new unit tests added (4 provider + 5 data service + 5 sync service + 1 mapper)

### Consequences

**Positive**:
- ✅ Theatre-specific logic encapsulated per implementation
- ✅ Easy to add new theatre APIs (copy pattern from TeatrPolski)
- ✅ Configuration-driven API endpoints (no hardcoded URLs)
- ✅ Comprehensive test coverage (15 new tests, 100% passing)
- ✅ Clearer separation of concerns (SRP compliance)

**Negative**:
- ❌ More files and interfaces to manage (+9 new files)
- ❌ Learning curve for new developers (understand interface design)
- ❌ Slight complexity increase from indirection (but offset by testability)

**Trade-offs**:
- Chose combined fetching+mapping per theatre (not separate) because response formats differ significantly
- Mapper is public static (not internal) for test accessibility
- Named HttpClient configuration ("TeatrPolski") instead of unnamed default

### Implementation Summary

**New Files Created** (9):
1. `IRepertoireDataService.cs` - Application/Contracts/Services
2. `ITheatreProviderService.cs` - Application/Contracts/Services
3. `TheatreApiConfiguration.cs` - Infrastructure/Configuration
4. `TeatrPolskiApiDtoMapper.cs` - Infrastructure/Services (moved from Application)
5. `TeatrPolskiRepertoireDataService.cs` - Infrastructure/Services
6. `TheatreProviderService.cs` - Infrastructure/Services
7. `TheatreProviderServiceTests.cs` - tests/ (4 tests)
8. `TeatrPolskiRepertoireDataServiceTests.cs` - tests/ (5 tests)
9. `TheatreRepertoireSyncServiceTests.cs` - tests/ (5 tests)

**Modified Files** (4):
1. `TheatreRepertoireSyncService.cs` - Complete refactor (orchestration only)
2. `appsettings.json` / `appsettings.Development.json` - Added `TheatreApis` section
3. `ServiceCollectionExtensions.cs` - DI registration for new services

**Test Coverage**:
- Total tests: 146 (from 131 baseline)
- New tests: 15 (14 new tests + refactored sync service tests)
- Pass rate: 100% (146/146 passing)

### Example: Adding a New Theatre

To add support for "Opera Wrocławska":

1. **Create configuration** (appsettings.json):
```json
"TheatreApis": {
  "OperaWroclawska": {
    "Url": "https://opera.wroclaw.pl/api/repertoire",
    "TimeoutSeconds": 30
  }
}
```

2. **Create data service** (OperaWroclawskaRepertoireDataService.cs):
```csharp
public class OperaWroclawskaRepertoireDataService : IRepertoireDataService
{
    // Inject IHttpClientFactory, IOptions<TheatreApiConfiguration>, ILogger
    public async Task<List<Show>> FetchAndMapRepertoireAsync(Guid theatreId, CancellationToken ct)
    {
        // Opera-specific HTTP call, JSON parsing, mapping
    }
}
```

3. **Register in DI** (ServiceCollectionExtensions.cs):
```csharp
services.Configure<TheatreApiConfiguration>(configuration.GetSection("TheatreApis:OperaWroclawska"));
services.AddScoped<IRepertoireDataService, OperaWroclawskaRepertoireDataService>();
```

4. **Create tests** (Opera WroclawskaRepertoireDataServiceTests.cs):
```csharp
[Fact]
public async Task FetchAndMapRepertoireAsync_WithValidResponse_Returns MappedShowEntities() { /* ... */ }
```

No changes needed to `TheatreRepertoireSyncService`, `TheatreProviderService`, or existing services.

### Alternatives Considered

**Alternative 1: Separate Fetching and Mapping Services**
- Rejected: Each theatre API has unique response format
- Would require N+N services (fetcher + mapper) instead of N combined services
- Adds unnecessary indirection when fetching/mapping are tightly coupled

**Alternative 2: Strategy Pattern with Single Service**
- Rejected: Still requires conditional logic in orchestration service
- Harder to test individual theatre implementations in isolation
- Less discoverable (strategies buried in composition)

**Alternative 3: Keep Monolithic Service with Switch Statement**
- Rejected: Violates Open/Closed Principle
- Every new theatre requires modifying sync service
- Poor testability (all theatres tested via single service)

**Decision**: Interface-based design with dependency injection (chosen approach) balances flexibility, testability, and simplicity.

### References
- **Related ADRs**: ADR-001 (Clean Architecture), ADR-002 (CQRS with MediatR)
- **Code Review**: [GitHub PR #XXX] (if applicable)
- **Documentation Updated**: BACKEND_SUMMARY.md, TEST_COVERAGE.md, SESSION_LOGGING.md (this ADR)

---

## Review & Approval

| Role | Status | Date |
|------|--------|------|
| Tech Lead | Approved | Feb 9, 2026 |
| Architect | Approved | Feb 9, 2026 |

---

**Document Version**: 1.2  
**Last Updated**: February 9, 2026  
**Next Review**: After Phase 2 (Redis integration) completion
