# Full Stack Setup Guide

Complete guide to running the Wrocław Theatre Tickets full-stack application.

## System Architecture

```
┌─────────────────────────────────────────────────────────┐
│  Browser (http://localhost:5173)                        │
│  ┌───────────────────────────────────────────────────┐  │
│  │  React Frontend (Vite Dev Server)                 │  │
│  │  - Theme Context (light/dark/auto)                │  │
│  │  - Auth Context (JWT token management)           │  │
│  │  - React Router (SPA routing)                     │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                          ↓ HTTP (Proxied /api → :5000)
┌─────────────────────────────────────────────────────────┐
│  Backend API (http://localhost:5000)                    │
│  ┌───────────────────────────────────────────────────┐  │
│  │  ASP.NET Core Minimal APIs                        │  │
│  │  - JWT Authentication                             │  │
│  │  - Rate Limiting                                  │  │
│  │  - CORS (AllowAll for dev)                        │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │  MediatR (CQRS)                                   │  │
│  │  - Show queries & commands                        │  │
│  │  - User commands                                  │  │
│  │  - Favorite management                            │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │  EF Core → SQLite                                 │  │
│  │  Database: theatre.db                             │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Prerequisites

### Required
- **.NET 10 SDK** - Backend runtime
  - Download: https://dotnet.microsoft.com/download/dotnet/10.0
  - Verify: `dotnet --version` (should show 10.x)

- **Node.js 18+** - Frontend tooling
  - Download: https://nodejs.org/ (LTS version)
  - Verify: `node --version` (should show v18.x or higher)
  - Verify: `npm --version`

### Optional
- **Visual Studio 2025** or **VS Code** with C# extension
- **Git** for version control

## Step-by-Step Setup

### 1. Backend Setup

Open a PowerShell terminal:

```powershell
# Navigate to project root
cd d:\Git\WroclawTheatreTickets

# Restore .NET dependencies
dotnet restore

# Build the backend
dotnet build WroclawTheatreTickets.slnx -c Release

# Run backend tests (optional)
dotnet test

# Start the backend API
cd src\WroclawTheatreTickets.Web
dotnet run
```

**Backend should now be running on:**
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

**Keep this terminal open!**

### 2. Frontend Setup

Open a **new** PowerShell terminal:

```powershell
# Navigate to client folder
cd d:\Git\WroclawTheatreTickets\client

# Install Node.js dependencies
npm install

# Start the frontend dev server
npm run dev
```

**Frontend should now be running on:**
- `http://localhost:5173`

**Keep this terminal open!**

### 3. Access the Application

Open your browser and navigate to:
- **Frontend**: http://localhost:5173
- **Backend Swagger**: https://localhost:5001/swagger

## Configuration

### Backend Configuration

Edit `src/WroclawTheatreTickets.Web/appsettings.Development.json`:

```json
{
  "Jwt": {
    "Secret": "your-secret-key-minimum-32-characters-long",
    "Issuer": "WroclawTheatreTickets",
    "Audience": "WroclawTheatreTickets.Users",
    "ExpirationInHours": 1
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=theatre.db"
  }
}
```

### Frontend Configuration

Edit `client/vite.config.ts` to change backend URL:

```typescript
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:5000', // Change this if backend runs elsewhere
      changeOrigin: true,
    },
  },
}
```

## User Guide

### Creating an Account

1. Click **Sign Up** in the left sidebar
2. Enter your email (will be your username)
3. Create a strong password:
   - At least 8 characters
   - At least 1 uppercase letter
   - At least 1 lowercase letter
   - At least 1 number
   - At least 1 special character
4. Click **Sign Up**

### Browsing Shows

**Home Page** (default view):
- Displays upcoming shows for the next 30 days
- Two-column layout
- Each card shows:
  - Show title
  - Theatre name and address
  - Date and time
  - Price range (if available)
  - Buy button (links to external ticketing)

**Show All Page**:
- Complete catalog of all shows
- Advanced filters:
  - Search by keyword
  - Filter by type (Opera, Ballet, etc.)
  - Price range (min/max)
  - Date range
  - Age restriction
- Click "Apply Filters" to search

### Managing Favorites

- **Add to favorites**: Click the star icon in top-right of any show card
- **Remove from favorites**: Click the star icon again
- **View all favorites**: Click "Favorites" in the sidebar

### Theme Switching

Click the theme buttons in the top-right header:
- ☀️ **Sun icon**: Light mode
- 🌙 **Moon icon**: Dark mode
- 🖥️ **Monitor icon**: Auto (follows system theme)

Your preference is saved and persists across sessions.

### Admin Features

If your account has Admin role:
1. "Admin" link appears in sidebar
2. Admin Dashboard shows:
   - Total shows statistics
   - Active shows count
   - Total views across all shows
   - Full show management list

## Troubleshooting

### Backend won't start

**Error: "Port 5000 is already in use"**
- Solution: Kill the process using port 5000
  ```powershell
  netstat -ano | findstr :5000
  taskkill /PID <process-id> /F
  ```

**Error: "Database connection failed"**
- Solution: Delete `theatre.db` and restart
  ```powershell
  cd src\WroclawTheatreTickets.Web
  rm theatre.db
  dotnet run
  ```

### Frontend won't start

**Error: "EADDRINUSE: Port 5173 is already in use"**
- Solution: Change port in `vite.config.ts` or kill process

**Error: "Module not found"**
- Solution: Reinstall dependencies
  ```powershell
  rm -rf node_modules package-lock.json
  npm install
  ```

### API calls fail (CORS errors)

**Error: "CORS policy: No 'Access-Control-Allow-Origin' header"**
- Ensure backend is running
- Check `Program.cs` has CORS enabled:
  ```csharp
  app.UseCors("AllowAll");
  ```

### Authentication issues

**Token expired or invalid:**
- Logout and login again
- Check browser console for errors
- Verify JWT secret is configured correctly in backend

**Can't login after registration:**
- Check backend logs for validation errors
- Ensure password meets requirements
- Verify email format is valid

## Development Workflow

### Making Backend Changes

1. Edit code in `src/WroclawTheatreTickets.*`
2. Backend auto-reloads with hot reload enabled
3. Or restart manually: `Ctrl+C` then `dotnet run`

### Making Frontend Changes

1. Edit code in `client/src/`
2. Vite hot-reloads automatically
3. See changes instantly in browser

### Running Tests

**Backend:**
```powershell
dotnet test                                    # All tests
dotnet test --filter "FullyQualifiedName~Show" # Show tests only
```

**Frontend:**
```powershell
npm test                                       # Run tests
npm run lint                                   # Check code quality
```

### Building for Production

**Backend:**
```powershell
dotnet publish -c Release -o ./publish
```

**Frontend:**
```powershell
npm run build                                  # Creates dist/ folder
npm run preview                                # Preview production build
```

## Production Deployment Checklist

- [ ] Update JWT secret to strong random value
- [ ] Configure production database (PostgreSQL/SQL Server)
- [ ] Set CORS to specific origins (not AllowAll)
- [ ] Enable HTTPS redirect
- [ ] Configure rate limiting appropriately
- [ ] Set up logging (Serilog to file/remote)
- [ ] Configure OAuth providers (Google, Facebook)
- [ ] Update frontend API base URL
- [ ] Build frontend for production
- [ ] Serve frontend through CDN or reverse proxy
- [ ] Set up health checks and monitoring
- [ ] Configure scheduled jobs (Quartz.NET)
- [ ] Test authentication flow end-to-end

## Additional Resources

- [Backend Documentation](../docs/BACKEND_SUMMARY.md)
- [Frontend README](../client/README.md)
- [Architecture Decisions](../docs/ARCHITECTURE_DECISIONS.md)
- [API Endpoints](../README.md#-api-documentation)
- [Test Coverage](../docs/TEST_COVERAGE.md)
