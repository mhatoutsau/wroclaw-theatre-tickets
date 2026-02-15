# Wroclaw Theatre Tickets

Full-stack application for discovering, filtering, and saving Wroclaw theatre performances. The solution follows Clean Architecture on the backend and a modern React + Vite frontend.

## Highlights

- Browse upcoming shows with search and advanced filters.
- Save favorites and manage user accounts with JWT auth.
- Role-based admin capabilities (review approval, dashboards).
- Automated parsing services ready for theatre data synchronization.
- Fast, isolated test suites across all backend layers.

## Tech Stack

**Backend**

- .NET 10, ASP.NET Core Minimal APIs
- Clean Architecture (Domain, Application, Infrastructure, Web)
- EF Core (SQLite for local cache)
- MediatR (CQRS), AutoMapper, FluentValidation
- JWT auth, Serilog logging, Quartz job scheduling

**Frontend**

- React 18 + TypeScript
- Vite, React Router, Tailwind CSS
- Axios API client, date-fns, Lucide icons

## Architecture

The backend is split into four layers with strict dependency flow:

- **Domain**: Entities, value objects, business rules
- **Application**: Use cases, DTOs, validators, interfaces
- **Infrastructure**: EF Core, repositories, external services
- **Web**: API endpoints, DI composition root, configuration

For the decision history, see [docs/ARCHITECTURE_DECISIONS.md](docs/ARCHITECTURE_DECISIONS.md).

## Repository Structure

```
backend/   # .NET backend (Clean Architecture)
frontend/  # React + Vite frontend
docs/      # Architecture, setup, and testing docs
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 18+

### Backend (API)

```powershell
cd backend

dotnet restore

dotnet build WroclawTheatreTickets.slnx -c Release

cd src/WroclawTheatreTickets.Web

dotnet run
```

Backend endpoints:

- API: http://localhost:5000/api
- Swagger: https://localhost:5001/swagger

### Frontend (Web App)

```powershell
cd frontend

npm install

npm run dev
```

Frontend dev server:

- http://localhost:5173

### Configuration Notes

- Backend configuration lives in [backend/src/WroclawTheatreTickets.Web/appsettings.json](backend/src/WroclawTheatreTickets.Web/appsettings.json).
- Frontend API proxy is configured in [frontend/vite.config.ts](frontend/vite.config.ts).

## Tests

### Backend

```powershell
cd backend

dotnet test
```

### Frontend

```powershell
cd frontend

npm test
```

Coverage details and current status are documented in [docs/TEST_COVERAGE.md](docs/TEST_COVERAGE.md).

## Documentation

- Backend overview: [docs/BACKEND_SUMMARY.md](docs/BACKEND_SUMMARY.md)
- Full stack setup: [docs/FULL_STACK_SETUP.md](docs/FULL_STACK_SETUP.md)
- Dependencies: [docs/DEPENDENCIES.md](docs/DEPENDENCIES.md)
- Session logging: [docs/SESSION_LOGGING.md](docs/SESSION_LOGGING.md)

## Session Logging (Required)

This repo uses automatic logging for AI-assisted sessions. Prompts and responses must be stored in `.chatlogs/prompts/` using the provided scripts or VS Code tasks. See [docs/SESSION_LOGGING.md](docs/SESSION_LOGGING.md) for details.

## Contributing

1. Follow the Clean Architecture dependency rules.
2. Keep new backend code consistent with the repository conventions.
3. Add tests for behavior changes.
4. Update docs when introducing new features.

## Status

This project includes a full backend implementation and a functional frontend with testing coverage. It is ready for local development and incremental feature work.
