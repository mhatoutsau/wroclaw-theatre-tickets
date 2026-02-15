---
date: 2026-02-09T11:50:04Z
role: assistant
conversation_id: unknown
context: WroclawTheatreTickets dto mapping
redacted: false
---

Perfect! All DTOs have been successfully updated with `JsonPropertyName` attributes. Here's what was changed:

**Updated DTOs with JsonPropertyName attributes:**
- `RepertoireApiResponse`: `events`
- `RepertoireEventDto`: All 12 properties (repertoireEventId, title, date, duration, paymentUrl, paymentDisabled, hiddenFromRepertoire, additionalProps, repertoireCategories, ageCategories, showEvent, stage)
- `RepertoireCategoryDto`: categoryId, title
- `AgeCategoryDto`: categoryId, title
- `ShowEventDto`: showEventId, slug, title, shared
- `StageDto`: stageId, name, buildingId, building
- `BuildingDto`: buildingId, name

The attributes now correctly map the PascalCase C# property names to the camelCase JSON property names used by the Theatre API, ensuring proper JSON deserialization.

Edits: src/WroclawTheatreTickets.Application/Contracts/Dtos/ApiDtos.cs
