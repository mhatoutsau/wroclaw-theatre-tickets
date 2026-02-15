---
date: 2026-02-09T14:01:00Z
role: assistant
conversation_id: theatre-sync-refactoring
context: Planning architecture for theatre sync service extraction
redacted: false
---

# Assistant Response: Architecture Planning

## Initial Analysis

Reviewed the monolithic `TheatreRepertoireSyncService` and identified 5+ responsibilities violating Single Responsibility Principle.

## Proposed Architecture

**Option 1: Separate Fetching and Mapping Services**
- `IDataFetchingService` - HTTP communication
- `IDataMappingService` - DTO to entity mapping
- Separate implementations per theatre

**Option 2: Combined Data Service per Theatre**
- `IRepertoireDataService` - Combined fetching + mapping
- Theatre-specific implementations (TeatrPolskiRepertoireDataService)
- Rationale: Each theatre has unique response format, tight coupling justified

## Clarification Questions Asked

1. Separate or combined fetching/mapping services?
2. Should API URLs move to configuration?
3. What to do with `ApiDtoMapper` (theatre-specific or shared)?
4. Return type: DTOs or domain entities?

## User Feedback Integration

- User chose **Option 2** (combined services)
- API URLs → configuration (appsettings.json)
- ApiDtoMapper → Infrastructure as TeatrPolski-specific
- Return type → Domain entities (Show)
