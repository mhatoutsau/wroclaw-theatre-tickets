# Session Logging - Quick Reference

## Quick Commands

### Log User Prompt (from clipboard)
```powershell
.\.scripts\save_prompt_enhanced.ps1 -Role user -Slug "description" -FromClipboard
```

### Log Assistant Response (from clipboard)
```powershell
.\.scripts\save_prompt_enhanced.ps1 -Role assistant -Slug "description" -FromClipboard
```

### Interactive Logging Menu
```powershell
.\.scripts\auto_capture.ps1
```

## VS Code Tasks

Press `Ctrl+Shift+P` → Type "run task" → Select:
- **Log User Prompt** - Captures from clipboard
- **Log Assistant Response** - Captures from clipboard
- **Log User Prompt (Interactive)** - Opens editor

## Optional Keyboard Shortcuts

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

## Workflow Example

1. Work with AI agent
2. Copy user prompt → `Ctrl+Alt+U` (or run task)
3. Copy assistant response → `Ctrl+Alt+A` (or run task)
4. Repeat for each interaction

## View Recent Logs

```powershell
Get-ChildItem .chatlogs\prompts\*.md | Sort-Object Name -Descending | Select-Object -First 10 Name
```

Or run `.\.scripts\auto_capture.ps1` and select option `[4]`.

## Full Documentation

See [docs/SESSION_LOGGING.md](../docs/SESSION_LOGGING.md) for complete guide.
