---
date: 2026-02-15T13:12:06.4390459+01:00
role: assistant
conversation_id: unknown
context: Frontend lint fixes
redacted: false
---

Removed unnecessary try/catch wrappers in AuthContext, memoized ShowAllPage load logic to satisfy exhaustive-deps, and suppressed react-refresh warnings for exported hooks. Re-ran frontend lint; it now passes aside from the TypeScript version support warning.

Changes:

- frontend/src/contexts/AuthContext.tsx
- frontend/src/contexts/ThemeContext.tsx
- frontend/src/pages/ShowAllPage.tsx

Tests:

- npm run lint (frontend/)
