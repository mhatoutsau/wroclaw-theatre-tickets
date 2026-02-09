# AI Session Logging Guide

## Overview

This workspace includes automatic session logging infrastructure to capture AI coding agent interactions. All prompts and responses are saved to `.chatlogs/prompts/` for audit trails, reproducibility, and team handoff.

## Quick Start

### Method 1: VS Code Tasks (Recommended)

1. **Copy content to clipboard** (user prompt or assistant response)
2. **Press** `Ctrl+Shift+P`
3. **Type** "run task" and select "Tasks: Run Task"
4. **Choose**:
   - `Log User Prompt` - for user requests
   - `Log Assistant Response` - for AI responses
5. **Enter** a short slug (e.g., "add-feature", "fix-bug")

### Method 2: Auto-Capture Script

Run the interactive logging tool:

```powershell
.\.scripts\auto_capture.ps1
```

Follow the menu to:
- Log prompts from clipboard
- Log responses from clipboard
- Use interactive editor for longer content
- View recent logs

### Method 3: Direct PowerShell

```powershell
# From clipboard
.\.scripts\save_prompt_enhanced.ps1 -Role user -Slug "add-feature" -FromClipboard

# Interactive editor
.\.scripts\save_prompt_enhanced.ps1 -Role user -Slug "add-feature" -Interactive

# Direct content
.\.scripts\save_prompt_enhanced.ps1 -Role assistant -Slug "feature-impl" -Content "Implementation details..."
```

## File Format

Each log file follows this structure:

```markdown
---
date: 2026-02-09T14:00:00Z
role: user|assistant
conversation_id: session-20260209-feature-work
context: Brief description of work area
redacted: false
---

<prompt or response content>
```

## Filename Convention

`YYYYMMDD_HHMMSS_role_slug.md`

Examples:
- `20260209_140000_user_add-feature.md`
- `20260209_140030_assistant_feature-impl.md`

## Advanced Usage

### Custom Context and Conversation ID

```powershell
.\.scripts\save_prompt_enhanced.ps1 `
  -Role user `
  -Slug "fix-auth" `
  -Content "Fix authentication bug" `
  -Context "Backend authentication module" `
  -ConversationId "session-auth-fixes"
```

### Automatic Redaction

The script automatically detects and redacts:
- Passwords (`password=...`)
- API keys (`apikey=...`, `api_key=...`)
- Secrets (`secret=...`)
- Tokens (`token=...`, `Bearer ...`)
- Connection strings (`connectionstring=...`)

Files with redacted content have `redacted: true` in front-matter.

## Optional: Keyboard Shortcuts

Add to `.vscode/keybindings.json`:

```json
[
  {
    "key": "ctrl+alt+u",
    "command": "workbench.action.tasks.runTask",
    "args": "Log User Prompt"
  },
  {
    "key": "ctrl+alt+a",
    "command": "workbench.action.tasks.runTask",
    "args": "Log Assistant Response"
  }
]
```

Then use:
- `Ctrl+Alt+U` - Log user prompt
- `Ctrl+Alt+A` - Log assistant response

## Viewing Logs

### In File Explorer
Browse to `.chatlogs/prompts/` - files are sorted chronologically by name.

### Using PowerShell
```powershell
# List recent logs
Get-ChildItem .chatlogs\prompts\*.md | Sort-Object Name -Descending | Select-Object -First 10 Name

# View a specific log
Get-Content .chatlogs\prompts\<filename>.md

# Search logs by content
Select-String -Path .chatlogs\prompts\*.md -Pattern "search term"
```

### In Auto-Capture Script
Run `.\.scripts\auto_capture.ps1` and select option `[4] View recent logs`.

## Best Practices

1. **Log immediately after each interaction** - Don't wait until end of session
2. **Use descriptive slugs** - Help future search and filtering
3. **Set meaningful context** - Specify which part of codebase you're working on
4. **Review before commit** - Check `.chatlogs/` is in `.gitignore` (it is)
5. **Redact manually if needed** - Script auto-redacts common patterns, but verify

## Integration with Copilot Instructions

The `.github/copilot-instructions.md` file includes guidance for AI agents to automatically log sessions. The infrastructure created here fulfills those requirements:

✓ Creates `.chatlogs/prompts/` folder  
✓ Uses YAML front-matter + markdown format  
✓ Implements automatic redaction  
✓ Provides multiple capture methods  
✓ Excluded from version control via `.gitignore`

## Backend-Specific Instructions

When editing backend code (`src/**`), review [`.github/instructions/backend.instructions.md`](../.github/instructions/backend.instructions.md) for language-specific conventions:

- Use single-line `using` statements instead of block statements
- Additional C# and project-specific rules

## Troubleshooting

### "Clipboard is empty" error
Ensure you've copied content to clipboard before running clipboard-based commands.

### Script execution policy error
Run once as administrator:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### VS Code task not found
Ensure `.vscode/tasks.json` exists and restart VS Code.

### Missing logs folder
The scripts auto-create `.chatlogs/prompts/` if it doesn't exist.

## Technical Details

- **Location**: `.chatlogs/prompts/`
- **Git ignore**: Yes (in `.gitignore`)
- **Format**: Markdown with YAML front-matter
- **Encoding**: UTF-8
- **Redaction**: Pattern-based automatic redaction
- **Scripts**: PowerShell 5.1+ (Windows default)

## Related Files

- `.scripts/save_prompt_enhanced.ps1` - Main logging script
- `.scripts/auto_capture.ps1` - Interactive logging interface
- `.scripts/save_prompt.ps1` - Legacy simple script (still works)
- `.vscode/tasks.json` - VS Code task definitions
- `.github/copilot-instructions.md` - AI agent guidance
