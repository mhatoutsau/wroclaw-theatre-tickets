# Wrocław Theatre Tickets - Frontend

React-based frontend application for the Wrocław Theatre Tickets system.

## Features

- **🎭 Browse Shows**: View upcoming theatre performances for the next 30 days
- **🔍 Search & Filter**: Advanced filtering by type, price, date, and age restriction
- **⭐ Favorites**: Save your favorite shows (requires authentication)
- **🌓 Theme Support**: Light, dark, and auto (system) theme modes
- **👤 User Authentication**: Sign up and login with email
- **👨‍💼 Role-Based Access**: Special features for moderators and administrators
- **📱 Responsive Design**: Works on desktop and mobile devices

## Technology Stack

- **React 18.3** - UI framework
- **TypeScript** - Type safety
- **Vite** - Fast build tool and dev server
- **React Router 6** - Client-side routing
- **Tailwind CSS 3** - Utility-first styling
- **Axios** - HTTP client
- **Lucide React** - Icon library
- **date-fns** - Date formatting

## Prerequisites

- Node.js 18+ or compatible runtime
- npm, pnpm, or yarn package manager
- Backend API running on `http://localhost:5000` (or configure proxy in vite.config.ts)

## Installation

```powershell
cd client
npm install
```

## Development

Start the development server:

```powershell
npm run dev
```

The application will be available at `http://localhost:5173`

API requests to `/api/*` are proxied to `http://localhost:5000` (configured in vite.config.ts)

## Testing

Run unit tests with Vitest:

```powershell
# Run all tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run tests with UI
npm run test:ui

# Generate coverage report
npm run test:coverage
```

See [TESTING.md](TESTING.md) for comprehensive testing guide.

### Test Coverage

Current test suite includes:

- ✅ **Component Tests**: ShowCard, Sidebar
- ✅ **Context Tests**: ThemeContext, AuthContext (with JWT decoding)
- ✅ **Page Tests**: HomePage
- ✅ **Mock Data**: Centralized test fixtures
- ✅ **Setup**: jsdom, localStorage mocks, matchMedia polyfill

## Building for Production

```powershell
npm run build
```

The production build will be created in the `dist/` folder.

Preview the production build:

```powershell
npm run preview
```

## Project Structure

```
client/
├── src/
│   ├── api/              # API client and HTTP utilities
│   │   └── client.ts     # Axios-based API client
│   ├── components/       # Reusable UI components
│   │   ├── Header.tsx    # Top header with theme switcher
│   │   ├── Layout.tsx    # Main layout wrapper
│   │   ├── ShowCard.tsx  # Show display card
│   │   └── Sidebar.tsx   # Left navigation menu
│   ├── contexts/         # React contexts
│   │   ├── AuthContext.tsx   # Authentication state
│   │   └── ThemeContext.tsx  # Theme management
│   ├── pages/            # Page components
│   │   ├── AdminPage.tsx     # Admin dashboard
│   │   ├── FavoritesPage.tsx # User favorites
│   │   ├── HomePage.tsx      # Home/upcoming shows
│   │   ├── LoginPage.tsx     # Login form
│   │   ├── ProfilePage.tsx   # User profile
│   │   ├── ShowAllPage.tsx   # All shows with filters
│   │   └── SignupPage.tsx    # Registration form
│   ├── types/            # TypeScript type definitions
│   │   └── api.ts        # API DTOs and interfaces
│   ├── App.tsx           # Main app component with routing
│   ├── main.tsx          # Application entry point
│   └── index.css         # Global styles and Tailwind
├── index.html            # HTML template
├── vite.config.ts        # Vite configuration
├── tailwind.config.js    # Tailwind CSS configuration
├── tsconfig.json         # TypeScript configuration
└── package.json          # Dependencies and scripts
```

## Key Features

### Authentication

- Email-based registration with strong password requirements
- JWT token-based authentication
- Automatic token storage in localStorage
- 401 handling with automatic redirect to login

### Theme System

- Three modes: Light, Dark, System
- Persists preference in localStorage
- Automatic system theme detection
- Smooth transitions between themes

### Show Management

- **Home Page**: Displays upcoming shows for the next 30 days in a two-column layout
- **Show All Page**: Complete catalog with advanced filters
- **Show Card**: Displays title, date, venue, price, and buy button
- **Favorites**: Star icon to add/remove from favorites (authenticated users only)

### Role-Based Features

- **User**: Browse shows, manage favorites, add reviews
- **Moderator**: Exclude shows from lists (future feature)
- **Admin**: Access to admin dashboard and analytics

## API Integration

The frontend communicates with the backend API at `/api`. Key endpoints:

- `GET /api/shows/upcoming?days=30` - Get upcoming shows
- `GET /api/shows` - Get all shows
- `POST /api/shows/filter` - Filter shows
- `GET /api/favorites` - Get user favorites
- `POST /api/favorites/:showId` - Add to favorites
- `DELETE /api/favorites/:showId` - Remove from favorites
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

## Environment Configuration

To change the backend API URL, edit `vite.config.ts`:

```typescript
server: {
  proxy: {
    '/api': {
      target: 'http://your-backend-url:port',
      changeOrigin: true,
    },
  },
}
```

## Contributing

When making changes:

1. Follow the existing code structure
2. Use TypeScript for all new files
3. Follow Tailwind CSS conventions for styling
4. Test authentication flows
5. Ensure responsive design works on mobile

## Future Enhancements

- Show details page with reviews
- Review submission and moderation interface
- Admin panel for show management
- User notification preferences
- Social sharing features
- Advanced search with autocomplete
