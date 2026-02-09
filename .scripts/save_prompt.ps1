param(
    [Parameter(Mandatory=$true)][ValidateSet('user','assistant')][string]$Role,
    [Parameter(Mandatory=$true)][string]$Slug,
    [Parameter(Mandatory=$true)][string]$Content
)

$dir = Join-Path -Path $PSScriptRoot -ChildPath "..\.chatlogs\prompts"
if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
$ts = (Get-Date).ToString('yyyyMMdd_HHmmss')
$file = Join-Path $dir "${ts}_${Role}_${Slug}.md"
@"
---
date: $(Get-Date -Format o)
role: $Role
conversation_id: session
context: auto-saved by save_prompt.ps1
redacted: false
---

$Content
"@ | Out-File -Encoding utf8 $file

Write-Output "Saved: $file"
