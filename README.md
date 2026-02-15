# 🎭 Wroclaw Theatre Tickets

<div align="center">

![Status](https://img.shields.io/badge/status-active-brightgreen?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet)
![React](https://img.shields.io/badge/React-18-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![License](https://img.shields.io/badge/license-MIT-blue?style=for-the-badge)

**Full-stack application for discovering, filtering, and saving Wroclaw theatre performances.**

_The solution follows Clean Architecture on the backend and a modern React + Vite frontend._

</div>

---

## ✨ Highlights

- ✅ Browse upcoming shows with search and advanced filters
- ✅ Save favorites and manage user accounts with JWT auth
- ✅ Role-based admin capabilities (review approval, dashboards)
- ✅ Automated parsing services ready for theatre data synchronization
- ✅ Fast, isolated test suites across all backend layers

## 🛠️ Tech Stack

### Backend

- 🚀 .NET 10, ASP.NET Core Minimal APIs
- 🏗️ Clean Architecture (Domain, Application, Infrastructure, Web)
- 💾 EF Core (SQLite for local cache)
- 📨 MediatR (CQRS), AutoMapper, FluentValidation
- 🔐 JWT auth, Serilog logging, Quartz job scheduling

### Frontend

- ⚛️ React 18 + TypeScript
- ⚡ Vite, React Router, Tailwind CSS
- 🌐 Axios API client, date-fns, Lucide icons

## 🏛️ Architecture

The backend is split into four layers with strict dependency flow:

| Layer                 | Description                                       |
| --------------------- | ------------------------------------------------- |
| 🎯 **Domain**         | Entities, value objects, business rules           |
| 💼 **Application**    | Use cases, DTOs, validators, interfaces           |
| 🔧 **Infrastructure** | EF Core, repositories, external services          |
| 🌐 **Web**            | API endpoints, DI composition root, configuration |

📖 For the decision history, see [docs/ARCHITECTURE_DECISIONS.md](docs/ARCHITECTURE_DECISIONS.md).

## 📁 Repository Structure

```
📂 backend/   # .NET backend (Clean Architecture)
📂 frontend/  # React + Vite frontend
📂 docs/      # Architecture, setup, and testing docs
```

## 🚀 Getting Started

### 📋 Prerequisites

- ✅ .NET 10 SDK
- ✅ Node.js 18+

### 🔧 Backend (API)

```powershell
cd backend

dotnet restore

dotnet build WroclawTheatreTickets.slnx -c Release

cd src/WroclawTheatreTickets.Web

dotnet run
```

**Backend endpoints:**

- 🌐 API: http://localhost:5000/api
- 📚 Swagger: https://localhost:5001/swagger

### ⚛️ Frontend (Web App)

```powershell
cd frontend

npm install

npm run dev
```

**Frontend dev server:**

- 🌐 http://localhost:5173

### ⚙️ Configuration Notes

- 🔧 Backend configuration lives in [backend/src/WroclawTheatreTickets.Web/appsettings.json](backend/src/WroclawTheatreTickets.Web/appsettings.json)
- 🔌 Frontend API proxy is configured in [frontend/vite.config.ts](frontend/vite.config.ts)

## 🧪 Tests

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

📊 Coverage details and current status are documented in [docs/TEST_COVERAGE.md](docs/TEST_COVERAGE.md).

## 🎯 Suggested Improvements

- [ ] Add end-to-end API tests for key endpoints (auth, shows, favorites)
- [ ] Introduce production database migrations (PostgreSQL/SQL Server)
- [ ] Add Redis caching for high-traffic queries and trending results
- [ ] Expand observability with OpenTelemetry traces and metrics dashboards
- [ ] Harden auth with refresh tokens and rate-limited endpoints

## 🗺️ Roadmap

| Milestone      | Status                                                              | Description                                        |
| -------------- | ------------------------------------------------------------------- | -------------------------------------------------- |
| 🎯 **Phase 1** | ![Planning](https://img.shields.io/badge/status-planning-yellow)    | API coverage and auth hardening                    |
| 🚀 **Phase 2** | ![Upcoming](https://img.shields.io/badge/status-upcoming-lightgrey) | Caching plus observability baseline                |
| 🏗️ **Phase 3** | ![Upcoming](https://img.shields.io/badge/status-upcoming-lightgrey) | Production database migration and operational docs |

## 📚 Documentation

| Document                                        | Description                                      |
| ----------------------------------------------- | ------------------------------------------------ |
| 📖 [Backend Overview](docs/BACKEND_SUMMARY.md)  | Detailed backend architecture and implementation |
| 🚀 [Full Stack Setup](docs/FULL_STACK_SETUP.md) | Complete setup instructions for both stacks      |
| 📦 [Dependencies](docs/DEPENDENCIES.md)         | Third-party packages and libraries               |
| 📝 [Session Logging](docs/SESSION_LOGGING.md)   | AI-assisted session logging guidelines           |

## 📝 Session Logging (Required)

> ⚠️ This repo uses automatic logging for AI-assisted sessions.

Prompts and responses must be stored in `.chatlogs/prompts/` using the provided scripts or VS Code tasks. See [docs/SESSION_LOGGING.md](docs/SESSION_LOGGING.md) for details.

## 🤝 Contributing

1. ✅ Follow the Clean Architecture dependency rules
2. ✅ Keep new backend code consistent with repository conventions
3. ✅ Add tests for behavior changes
4. ✅ Update docs when introducing new features

## 📊 Project Status

<div align="center">

| Component     | Status                                                                |
| ------------- | --------------------------------------------------------------------- |
| Backend API   | ![Complete](https://img.shields.io/badge/status-complete-brightgreen) |
| Frontend UI   | ![Complete](https://img.shields.io/badge/status-complete-brightgreen) |
| Test Coverage | ![Good](https://img.shields.io/badge/coverage-good-green)             |
| Documentation | ![Complete](https://img.shields.io/badge/docs-complete-brightgreen)   |

**This project is ready for local development and incremental feature work! 🎉**

</div>

---

<div align="center">

Made with ❤️ for Wroclaw Theatre Community

</div>
