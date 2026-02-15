---
date: 2026-02-09T13:09:00Z
role: assistant
conversation_id: session-wroclaw-theatre-cleanup
context: Fixed MediatR registration to scan Application assembly for handlers
redacted: false
---

## MediatR Handler Registration Fixed ✅

**Problem:** MediatR was scanning the wrong assembly (Web) for handlers, but all handlers are in the Application assembly.

**Root Cause:** Registration was using:
```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ServiceCollectionExtensions)));
```
This scanned the Web assembly, but handlers like `SaveOrUpdateShowCommandHandler` are in Application.

**Solution:** Updated registration to scan Application assembly:
```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SaveOrUpdateShowCommand>());
```

### Files Modified

1. **ServiceCollectionExtensions.cs** — Updated MediatR registration
   - Added using: `WroclawTheatreTickets.Application.UseCases.Shows.Commands`
   - Changed assembly scan from Web to Application assembly

### What This Fixes

MediatR will now discover and register all `IRequestHandler<>` implementations from:
- `/Application/UseCases/Shows/Commands/` — Command handlers
- `/Application/UseCases/Shows/Queries/` — Query handlers
- `/Application/UseCases/Favorites/` — Favorite handlers
- `/Application/UseCases/Reviews/` — Review handlers
- `/Application/UseCases/Users/` — User handlers

### Build Note

Build currently blocked by running process locking DLLs. Stop the running application (`WroclawTheatreTickets.Web (26500)`) before rebuilding.

**Next Steps:**
1. Stop any running instances of the application
2. Build cleanly with: `dotnet build WroclawTheatreTickets.slnx`
3. Run application to verify handlers are now resolved correctly
