---
date: 2026-02-15T13:08:36.9680843+01:00
role: assistant
conversation_id: unknown
context: Frontend tests
redacted: false
---

Fixed frontend test failures by improving the localStorage mock, adding an accessible loading spinner role, and aligning auth/date test data with actual formatting. Re-ran the frontend tests in non-watch mode; all 40 tests passed.

Changes:

- frontend/src/test/setup.ts
- frontend/src/pages/HomePage.tsx
- frontend/src/test/mockData.ts
- frontend/src/pages/HomePage.test.tsx
- frontend/src/components/ShowCard.test.tsx

Tests:

- npm run test -- --run (frontend/)

Notes:

- React Router future flag warnings still appear in stderr but do not fail tests.
