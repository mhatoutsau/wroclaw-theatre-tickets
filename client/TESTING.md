# Frontend Testing Guide

This document describes the testing setup and practices for the Wrocław Theatre Tickets frontend.

## Testing Stack

- **Vitest** - Fast unit test framework (Vite-native)
- **React Testing Library** - Component testing utilities
- **jsdom** - DOM implementation for Node.js
- **@testing-library/jest-dom** - Custom matchers for DOM testing
- **@testing-library/user-event** - User interaction simulation

## Running Tests

```bash
# Run all tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run tests with UI
npm run test:ui

# Generate coverage report
npm run test:coverage
```

## Test Structure

```
client/src/
├── components/
│   ├── ShowCard.tsx
│   └── ShowCard.test.tsx          # Component tests
├── contexts/
│   ├── AuthContext.tsx
│   ├── AuthContext.test.tsx       # Context tests
│   ├── ThemeContext.tsx
│   └── ThemeContext.test.tsx
├── pages/
│   ├── HomePage.tsx
│   └── HomePage.test.tsx          # Page tests
└── test/
    ├── setup.ts                   # Test configuration
    └── mockData.ts                # Mock data for tests
```

## Writing Tests

### Component Tests

Test components using React Testing Library:

```typescript
import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import { MyComponent } from './MyComponent'

describe('MyComponent', () => {
  it('renders correctly', () => {
    render(
      <BrowserRouter>
        <MyComponent />
      </BrowserRouter>
    )
    
    expect(screen.getByText('Expected Text')).toBeInTheDocument()
  })
})
```

### Context Tests

Test contexts using `renderHook`:

```typescript
import { renderHook, act } from '@testing-library/react'
import { ThemeProvider, useTheme } from './ThemeContext'

it('changes theme', () => {
  const { result } = renderHook(() => useTheme(), {
    wrapper: ThemeProvider,
  })

  act(() => {
    result.current.setTheme('dark')
  })

  expect(result.current.theme).toBe('dark')
})
```

### Testing with Auth and Theme

Use the helper function to wrap components with providers:

```typescript
const renderWithProviders = (component: React.ReactElement) => {
  return render(
    <BrowserRouter>
      <ThemeProvider>
        <AuthProvider>
          {component}
        </AuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  )
}

// Then use it in tests
renderWithProviders(<MyComponent />)
```

### Mocking API Calls

Mock the API client using Vitest:

```typescript
import { vi } from 'vitest'
import * as apiClient from '../api/client'

vi.mock('../api/client', () => ({
  apiClient: {
    getUpcomingShows: vi.fn(),
    addFavorite: vi.fn(),
  },
}))

// In your test
vi.mocked(apiClient.apiClient.getUpcomingShows).mockResolvedValue(mockShows)
```

## Test Coverage Goals

Current coverage targets:

- **Statements**: 80%+
- **Branches**: 75%+
- **Functions**: 80%+
- **Lines**: 80%+

View coverage report:

```bash
npm run test:coverage
open coverage/index.html
```

## Test Files Created

### Component Tests

1. **ShowCard.test.tsx**
   - Renders show information correctly
   - Displays buy button when URL provided
   - Formats prices correctly
   - Handles favorite functionality
   - Tests image display and truncation

2. **Sidebar.test.tsx**
   - Displays logo and navigation
   - Shows appropriate auth buttons
   - Highlights active routes
   - Conditional rendering based on auth state

### Context Tests

3. **ThemeContext.test.tsx**
   - Default theme handling
   - Theme switching (light/dark/system)
   - localStorage persistence
   - Theme loading on mount
   - Document class updates

4. **AuthContext.test.tsx**
   - Initial unauthenticated state
   - Token loading from storage
   - Login functionality
   - Registration functionality
   - Logout functionality
   - Role detection (User/Moderator/Admin)
   - JWT token decoding with .NET ClaimTypes

### Page Tests

5. **HomePage.test.tsx**
   - Loading state display
   - Shows rendering after load
   - Error handling
   - Empty state display
   - Grid layout verification
   - Retry button on error

## Mock Data

Mock data is centralized in `src/test/mockData.ts`:

- `mockShow` - Single show object
- `mockShows` - Array of shows
- `mockAuthResponse` - Authentication response

## Best Practices

### Do's

✅ Test user-visible behavior, not implementation details
✅ Use semantic queries (`getByRole`, `getByLabelText`, `getByText`)
✅ Test accessibility
✅ Mock external dependencies (API calls, browser APIs)
✅ Test error states and edge cases
✅ Use `waitFor` for async operations
✅ Clean up after each test

### Don'ts

❌ Don't test implementation details (state, props)
❌ Don't use `querySelector` unless necessary
❌ Don't forget to mock API calls
❌ Don't test library code
❌ Don't rely on test execution order
❌ Don't forget to test loading and error states

## Continuous Integration

Tests run automatically on:

- Pre-commit (via git hooks)
- Pull request creation
- Main branch commits

## Debugging Tests

### VS Code

Add to `.vscode/launch.json`:

```json
{
  "type": "node",
  "request": "launch",
  "name": "Debug Vitest Tests",
  "runtimeExecutable": "npm",
  "runtimeArgs": ["run", "test"],
  "console": "integratedTerminal",
  "internalConsoleOptions": "neverOpen"
}
```

### Chrome DevTools

```bash
node --inspect-brk ./node_modules/.bin/vitest --run
```

Then open `chrome://inspect` in Chrome.

## Common Issues

### localStorage not defined

Fixed in `src/test/setup.ts` with mock implementation.

### window.matchMedia not defined

Fixed in `src/test/setup.ts` with mock implementation.

### Router errors

Always wrap components that use routing in `<BrowserRouter>`.

### Context errors

Always wrap components that use contexts with their providers.

## Adding New Tests

When adding new features:

1. Create test file alongside source file: `Component.test.tsx`
2. Follow existing test patterns
3. Add mock data to `mockData.ts` if needed
4. Run tests and verify coverage
5. Update this document if introducing new patterns

## Resources

- [Vitest Documentation](https://vitest.dev/)
- [React Testing Library](https://testing-library.com/react)
- [Testing Library Best Practices](https://kentcdodds.com/blog/common-mistakes-with-react-testing-library)
- [Vitest UI](https://vitest.dev/guide/ui.html)
