---
date: 2026-02-09T12:00:05Z
role: assistant
conversation_id: unknown
context: WroclawTheatreTickets repo
redacted: false
---

Split repository contracts into individual files under [src/WroclawTheatreTickets.Application/Contracts/Repositories](src/WroclawTheatreTickets.Application/Contracts/Repositories), and removed the combined file [src/WroclawTheatreTickets.Application/Contracts/Repositories/IRepositories.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/IRepositories.cs).

- New files: [src/WroclawTheatreTickets.Application/Contracts/Repositories/ITheatreRepository.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/ITheatreRepository.cs), [src/WroclawTheatreTickets.Application/Contracts/Repositories/IShowRepository.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/IShowRepository.cs), [src/WroclawTheatreTickets.Application/Contracts/Repositories/ShowFilterCriteria.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/ShowFilterCriteria.cs), [src/WroclawTheatreTickets.Application/Contracts/Repositories/IUserRepository.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/IUserRepository.cs), [src/WroclawTheatreTickets.Application/Contracts/Repositories/IFavoriteRepository.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/IFavoriteRepository.cs), [src/WroclawTheatreTickets.Application/Contracts/Repositories/IReviewRepository.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/IReviewRepository.cs), [src/WroclawTheatreTickets.Application/Contracts/Repositories/IViewHistoryRepository.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/IViewHistoryRepository.cs), [src/WroclawTheatreTickets.Application/Contracts/Repositories/INotificationRepository.cs](src/WroclawTheatreTickets.Application/Contracts/Repositories/INotificationRepository.cs).
- Tests not run (not requested).

Next steps if you want a quick check:
1) Run `dotnet build WroclawTheatreTickets.slnx`
2) Run `dotnet test`
