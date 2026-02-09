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

## Review & Approval

| Role | Status | Date |
|------|--------|------|
| Tech Lead | Approved | Feb 9, 2025 |
| Architect | Approved | Feb 9, 2025 |

---

**Document Version**: 1.0  
**Last Updated**: February 9, 2025  
**Next Review**: After Phase 1 completion
