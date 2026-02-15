---
date: 2026-02-09T13:03:00Z
role: assistant
conversation_id: session-wroclaw-theatre-cleanup
context: Added automatic session logging instructions to copilot-instructions.md
redacted: false
---

## Automatic Session Logging Instructions Added

Updated `.github/copilot-instructions.md` with a new **Automatic Session Logging** section that provides clear, actionable guidance for AI agents to automatically save all prompts and responses.

### Key Instructions Added

1. **REQUIREMENT**: Every AI agent session MUST automatically log user prompts and assistant responses
2. **Timing**: Save logs AFTER each request is completed
3. **Automation**: Treat as automatic step — don't ask for permission
4. **Workflow**: Create prompt file after user request, then response file after completing work
5. **File Format**: YAML front-matter + markdown body with timestamps
6. **Error Recovery**: Don't block main work if logging fails
7. **Privacy**: Redact secrets and set `redacted: true` flag
8. **Benefits**: Audit trail, AI handoff, debugging, decision history

### Verification

- ✅ `.chatlogs/` folder already in `.gitignore` (won't clutter repository)
- ✅ Instructions placed prominently before "Save prompts & chat logs" section
- ✅ Clear workflow and examples provided

### Implementation Notes

All future agent sessions will now have clear, explicit instructions to maintain session logs automatically, enabling:
- **Reproducibility**: Full audit trail of all work
- **Handoff**: Seamless transition between agent sessions
- **Debugging**: Historical context for issue investigation
- **Documentation**: Automatic decision history
