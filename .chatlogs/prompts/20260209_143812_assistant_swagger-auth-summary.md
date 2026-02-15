---
date: 2026-02-09T11:50:08Z
role: assistant
conversation_id: unknown
context: WroclawTheatreTickets swagger auth
redacted: false
---

Perfect! I've successfully added JWT Bearer authentication to Swagger. Here's what was configured:

## Changes Made:

### 1. Added Required Import
- Added `Microsoft.OpenApi.Models` namespace to [ServiceCollectionExtensions.cs](src/WroclawTheatreTickets.Web/ServiceCollectionExtensions.cs#L6)

### 2. Enhanced Swagger Configuration
The Swagger configuration now includes:

**API Documentation:**
- Title: "Wrocław Theatre Tickets API"
- Version: "v1"
- Description and contact information

**JWT Bearer Authentication:**
- Security definition named "Bearer" with HTTP bearer scheme
- Configured to use JWT format
- Added helpful description for users on how to enter the token
- Applied security requirement globally to all endpoints

## How to Use:

1. **Run the application** in development mode
2. **Navigate to Swagger UI** (typically at `/swagger`)
3. **Click the "Authorize" button** (lock icon) at the top right
4. **Enter your JWT token** in the format: `Bearer <your-token-here>`
5. **Click "Authorize"** and then "Close"
6. All subsequent API requests will include the authentication header

The build completed successfully with no errors related to the changes!

Edits: src/WroclawTheatreTickets.Web/ServiceCollectionExtensions.cs
