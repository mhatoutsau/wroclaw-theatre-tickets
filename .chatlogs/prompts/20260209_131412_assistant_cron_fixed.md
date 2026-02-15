---
date: 2026-02-09T13:07:00Z
role: assistant
conversation_id: session-wroclaw-theatre-cleanup
context: Fixed Quartz.NET cron expression format error
redacted: false
---

## Cron Expression Fixed ✅

**Problem:** Invalid cron expressions with 7 fields causing `System.FormatException`

**Root Cause:** The cron expressions had an extra asterisk at the end:
- `"0 0 2 ? * * *"` (7 fields - invalid)
- `"0 0 3 ? * SUN *"` (7 fields - invalid)

**Solution:** Fixed to use standard 6-field Quartz.NET cron format:
- `"0 0 2 ? * *"` - Daily at 2:00 AM
- `"0 0 3 ? * SUN"` - Weekly on Sunday at 3:00 AM

### Quartz.NET Cron Format (6 fields)

```
second minute hour day-of-month month day-of-week
  0      0     2        ?         *        *        = Daily at 2 AM
  0      0     3        ?         *       SUN       = Sunday at 3 AM
```

**Key Rules:**
- Must use `?` for either day-of-month OR day-of-week (not both)
- Cannot use `*` for both day-of-month and day-of-week
- 6 fields is standard; 7th field (year) is optional

**Build Status:** ✅ Succeeded in 1.7s - Application now starts without cron format errors
