---
date: 2026-02-09T12:04:05Z
role: assistant
conversation_id: unknown
context: WroclawTheatreTickets repo
redacted: false
---

Split service contracts and sync types into individual files under [src/WroclawTheatreTickets.Application/Contracts/Services](src/WroclawTheatreTickets.Application/Contracts/Services), and removed the combined files [src/WroclawTheatreTickets.Application/Contracts/Services/IServices.cs](src/WroclawTheatreTickets.Application/Contracts/Services/IServices.cs) and [src/WroclawTheatreTickets.Application/Contracts/Services/ISyncService.cs](src/WroclawTheatreTickets.Application/Contracts/Services/ISyncService.cs).

- New files: [src/WroclawTheatreTickets.Application/Contracts/Services/IAuthenticationService.cs](src/WroclawTheatreTickets.Application/Contracts/Services/IAuthenticationService.cs), [src/WroclawTheatreTickets.Application/Contracts/Services/IEmailService.cs](src/WroclawTheatreTickets.Application/Contracts/Services/IEmailService.cs), [src/WroclawTheatreTickets.Application/Contracts/Services/INotificationService.cs](src/WroclawTheatreTickets.Application/Contracts/Services/INotificationService.cs), [src/WroclawTheatreTickets.Application/Contracts/Services/ICacheService.cs](src/WroclawTheatreTickets.Application/Contracts/Services/ICacheService.cs), [src/WroclawTheatreTickets.Application/Contracts/Services/ITheatreRepertoireSyncService.cs](src/WroclawTheatreTickets.Application/Contracts/Services/ITheatreRepertoireSyncService.cs), [src/WroclawTheatreTickets.Application/Contracts/Services/TheatreSyncResult.cs](src/WroclawTheatreTickets.Application/Contracts/Services/TheatreSyncResult.cs).
- Tests not run (not requested).
