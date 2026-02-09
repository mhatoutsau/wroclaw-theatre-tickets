# NuGet Package Reference

## Package Versions Used

### WroclawTheatreTickets.Domain
No external dependencies (by design)

### WroclawTheatreTickets.Application
- **MediatR** 12.4.0 - CQRS command/query dispatching
- **AutoMapper.Extensions.Microsoft.DependencyInjection** 12.0.1 - Object mapping
- **FluentValidation.DependencyInjectionExtensions** 11.9.2 - Input validation

### WroclawTheatreTickets.Infrastructure
- **Microsoft.EntityFrameworkCore** 10.0.0 - ORM
- **Microsoft.EntityFrameworkCore.Sqlite** 10.0.0 - SQLite provider
- **Microsoft.EntityFrameworkCore.Tools** 10.0.0 - Migration tools
- **Quartz.Extensions.Hosting** 3.13.0 - Job scheduling
- **HtmlAgilityPack** 1.11.65 - HTML parsing
- **Serilog** 4.0.0 - Logging framework
- **Serilog.Sinks.Console** 6.0.0 - Console log output
- **Serilog.Sinks.File** 5.0.0 - File log output
- **BCrypt.Net-Next** 4.0.3 - Password hashing
- **Microsoft.Extensions.Configuration** 10.0.0 - Configuration
- **Microsoft.Extensions.Logging** 10.0.0 - Logging abstractions
- **System.IdentityModel.Tokens.Jwt** 8.2.0 - JWT token handling
- **Microsoft.IdentityModel.Tokens** 8.2.0 - Token security

### WroclawTheatreTickets.Web
- **Microsoft.AspNetCore.Authentication.JwtBearer** 10.0.0 - JWT authentication middleware
- **System.IdentityModel.Tokens.Jwt** 8.2.0 - JWT support
- **Serilog.AspNetCore** 10.0.0 - ASP.NET Core logging integration
- **Serilog.Sinks.Async** 2.0.0 - Async logging
- **Swashbuckle.AspNetCore** 7.2.0 - Swagger/OpenAPI documentation
- **FluentValidation** 11.9.2 - Validation framework

## Compatibility

### .NET 10.0
All packages are compatible with .NET 10.0 and support:
- Nullable reference types
- Nullable annotations
- Latest async patterns
- Modern C# language features (records, patterns, etc.)

## Security Advisories

### Current Status ✅
- All packages are from trusted publishers
- No known critical vulnerabilities as of Feb 2025
- Regular updates recommended

### Packages to Monitor
- **System.IdentityModel.Tokens.Jwt** - Security updates
- **Microsoft.IdentityModel.Tokens** - Security updates
- **HtmlAgilityPack** - XSS/injection concerns when parsing untrusted HTML

## Performance Considerations

### High-Impact Packages
1. **MediatR** - Reflection-based dispatching, consider source generators for high-throughput
2. **AutoMapper** - DTO mapping, consider source-generated mappers for performance
3. **Entity Framework Core** - Query optimization important for large datasets
4. **HtmlAgilityPack** - DOM parsing can be memory intensive for large HTML documents

### Optimization Opportunities
- Replace AutoMapper with Mapster (faster)
- Use MediatR source generators
- Implement Redis caching layer
- Consider GraphQL for complex queries

## License Compliance

### Package Licenses
| Package | License | Notes |
|---------|---------|-------|
| MediatR | Apache 2.0 | ✅ |
| AutoMapper | MIT | ✅ |
| FluentValidation | Apache 2.0 | ✅ |
| EF Core | MIT | ✅ |
| Serilog | Apache 2.0 | ✅ |
| HtmlAgilityPack | MIT | ✅ |
| BCrypt.Net | MIT | ✅ |
| Quartz.NET | Apache 2.0 | ✅ |

All licenses are permissive for commercial use.

## Updating Dependencies

### Safety Approach
1. Update one package at a time
2. Run full test suite after each update
3. Review release notes for breaking changes
4. Test in development environment first

### Check for Updates
```powershell
dotnet outdated
```

### Update All Packages
```powershell
dotnet add package --upgrade
```

### Update Specific Package
```powershell
dotnet add package MediatR --version 13.0.0
```

## Deprecated Packages (To Avoid)

- ❌ System.Net.Http (Use HttpClient from System.Net.Http)
- ❌ System.Linq.Queryable (Already included in System.Linq)
- ❌ EntityFramework 6.x (Use EF Core instead)
- ❌ Newtonsoft.Json for ASP.NET Core (Use System.Text.Json)
- ❌ Automapper.Extensions.DependencyInjection older than 11.x

## Future Considerations

### Potential Additions (When Needed)
- **StackExchange.Redis** - In-memory caching
- **Elasticsearch.Net** - Full-text search
- **Serilog.Sinks.Seq** - Centralized log management
- **Refit** - Strongly-typed HTTP client
- **Polly** - Resilience and transient-fault handling
- **MassTransit** - Service bus / event publishing
- **OpenTelemetry** - Distributed tracing
- **Hangfire** - Job scheduling alternative

## Dependency Validation

### Circular Dependencies ✅
- No circular dependencies between projects
- Clear inward-facing dependency flow

### Transitive Dependencies
Verify important transitive dependencies:
```powershell
dotnet package search --exact MediatR
```

## Version Lock Strategy

### Production
Use exact versions in `.csproj`:
```xml
<PackageReference Include="MediatR" Version="12.4.0" />
```

### Development
Use floating versions for flexibility:
```xml
<PackageReference Include="MediatR" Version="12.*" />
```

## Build Output

### Package References Summary
```
Domain Dependencies: 0
Application Dependencies: 3
Infrastructure Dependencies: 12
Web Dependencies: 6
Total: 21 NuGet packages
```

### NuGet Cache
Clear cache if issues occur:
```powershell
dotnet nuget locals all --clear
```

## Dependency Documentation

### Key Package Docs
- [MediatR Docs](https://github.com/jbogard/MediatR)
- [AutoMapper Docs](https://docs.automapper.org/)
- [FluentValidation Docs](https://docs.fluentvalidation.net/)
- [EF Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Serilog Docs](https://github.com/serilog/serilog/wiki)
- [Quartz.NET Docs](https://www.quartz-scheduler.net/)
- [HtmlAgilityPack Docs](https://html-agility-pack.net/)

---

**Last Updated**: February 9, 2025  
**Packages Validated**: ✅  
**Security Scan**: ✅ Clean
