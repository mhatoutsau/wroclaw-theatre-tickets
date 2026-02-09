<#
.SYNOPSIS
Enhanced prompt logging script for AI coding sessions

.DESCRIPTION
Saves user prompts and assistant responses to .chatlogs/prompts/ folder
with YAML front-matter and markdown body for audit trail and session continuity.

.PARAMETER Role
The role: 'user' or 'assistant'

.PARAMETER Slug
Short descriptive slug for the filename (e.g., 'add-logging', 'fix-bug')

.PARAMETER Content
The content to save. If not provided, will attempt to read from clipboard.

.PARAMETER Context
Optional context description (e.g., 'Backend API development')

.PARAMETER ConversationId
Optional conversation/session identifier

.PARAMETER FromClipboard
If specified, reads content from clipboard instead of Content parameter

.PARAMETER Interactive
If specified, opens an editor to input content interactively

.EXAMPLE
.\save_prompt_enhanced.ps1 -Role user -Slug "add-logging" -Content "Add session logging"

.EXAMPLE
.\save_prompt_enhanced.ps1 -Role assistant -Slug "logging-impl" -FromClipboard

.EXAMPLE
.\save_prompt_enhanced.ps1 -Role user -Slug "fix-issue" -Interactive
#>

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('user','assistant')]
    [string]$Role,
    
    [Parameter(Mandatory=$true)]
    [string]$Slug,
    
    [Parameter(Mandatory=$false)]
    [string]$Content = "",
    
    [Parameter(Mandatory=$false)]
    [string]$Context = "WroclawTheatreTickets development",
    
    [Parameter(Mandatory=$false)]
    [string]$ConversationId = "session-$(Get-Date -Format 'yyyyMMdd')",
    
    [switch]$FromClipboard,
    
    [switch]$Interactive
)

# Determine content source
if ($FromClipboard) {
    try {
        $Content = Get-Clipboard -Raw
        if ([string]::IsNullOrWhiteSpace($Content)) {
            Write-Error "Clipboard is empty"
            exit 1
        }
        Write-Host "Content read from clipboard ($(($Content -split '\n').Count) lines)" -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to read clipboard: $_"
        exit 1
    }
}
elseif ($Interactive) {
    $tempFile = [System.IO.Path]::GetTempFileName() + ".md"
    "# Enter content below this line`n`n" | Out-File -Encoding utf8 $tempFile
    
    # Open in default editor
    & notepad.exe $tempFile
    Wait-Process -Name notepad
    
    $Content = Get-Content -Path $tempFile -Raw
    $Content = $Content -replace '^# Enter content below this line\s+', ''
    Remove-Item $tempFile -ErrorAction SilentlyContinue
    
    if ([string]::IsNullOrWhiteSpace($Content)) {
        Write-Error "No content provided"
        exit 1
    }
}
elseif ([string]::IsNullOrWhiteSpace($Content)) {
    Write-Error "Content is required. Use -Content, -FromClipboard, or -Interactive"
    exit 1
}

# Ensure directory exists
$dir = Join-Path -Path $PSScriptRoot -ChildPath "..\.chatlogs\prompts"
if (-not (Test-Path $dir)) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
}

# Generate filename with timestamp
$ts = (Get-Date).ToString('yyyyMMdd_HHmmss')
$file = Join-Path $dir "${ts}_${Role}_${Slug}.md"

# Check for redaction (basic pattern matching)
$redacted = $false
$redactionPatterns = @(
    'password\s*[:=]\s*\S+',
    'apikey\s*[:=]\s*\S+',
    'api[_-]?key\s*[:=]\s*\S+',
    'secret\s*[:=]\s*\S+',
    'token\s*[:=]\s*\S+',
    'connectionstring\s*[:=]\s*\S+',
    'Bearer\s+[A-Za-z0-9\-\._~\+\/]+=*'
)

foreach ($pattern in $redactionPatterns) {
    if ($Content -match $pattern) {
        $Content = $Content -replace $pattern, '<REDACTED>'
        $redacted = $true
    }
}

if ($redacted) {
    Write-Warning "Sensitive information detected and redacted"
}

# Build file content
$fileContent = @"
---
date: $(Get-Date -Format o)
role: $Role
conversation_id: $ConversationId
context: $Context
redacted: $redacted
---

$Content
"@

# Write to file
$fileContent | Out-File -Encoding utf8 $file

Write-Host "Saved: " -NoNewline
Write-Host $file -ForegroundColor Cyan
Write-Host "  Role: $Role | Slug: $Slug | Lines: $(($Content -split '\n').Count)" -ForegroundColor Gray

# Return file path for pipeline usage
return $file
