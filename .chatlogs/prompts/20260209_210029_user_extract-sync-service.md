---
date: 2026-02-09T14:00:00Z
role: user
conversation_id: theatre-sync-refactoring
context: WroclawTheatreTickets backend - refactoring monolithic sync service
redacted: false
---

# User Request: Extract Theatre Sync Service

Extract current implementation of the sync server to a new fetching service and then update TheatreRepertoireService to use multiple fetching services. Create fetching service interface and register it in a DI. Create unit tests for the fetching services. Update documentation. Save prompts.

## Context Provided

User provided the existing `TheatreRepertoireSyncService.cs` file showing a monolithic service with multiple responsibilities:
- HTTP communication with Teatr Polski API
- JWT token parsing
- DTO to domain entity mapping
- Theatre entity management (get-or-create)
- MediatR command orchestration

## User Intent

Separate concerns to follow SOLID principles and support multiple theatre APIs in the future.
