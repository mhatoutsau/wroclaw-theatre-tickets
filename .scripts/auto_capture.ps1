<#
.SYNOPSIS
Auto-capture helper for continuous session logging

.DESCRIPTION
Provides a simple workflow for capturing prompts and responses during an AI coding session.
Prompts for role and slug, then captures content from clipboard.

.EXAMPLE
.\auto_capture.ps1

Then follow the interactive prompts to log user prompts or assistant responses.
#>

Write-Host @"
================================
 AI Session Auto-Logger
================================
"@ -ForegroundColor Cyan

Write-Host "`nThis script helps you quickly log AI session interactions.`n" -ForegroundColor Gray

function Show-Menu {
    Write-Host "Select action:" -ForegroundColor Yellow
    Write-Host "  [1] Log User Prompt (from clipboard)"
    Write-Host "  [2] Log Assistant Response (from clipboard)"
    Write-Host "  [3] Log User Prompt (interactive editor)"
    Write-Host "  [4] View recent logs"
    Write-Host "  [Q] Quit`n"
}

function Get-RecentLogs {
    $logDir = Join-Path -Path $PSScriptRoot -ChildPath "..\.chatlogs\prompts"
    if (Test-Path $logDir) {
        $logs = Get-ChildItem -Path $logDir -Filter "*.md" | Sort-Object Name -Descending | Select-Object -First 10
        Write-Host "`nRecent logs:" -ForegroundColor Green
        foreach ($log in $logs) {
            Write-Host "  $($log.Name)" -ForegroundColor Gray
        }
    }
    else {
        Write-Host "No logs found." -ForegroundColor Yellow
    }
    Write-Host ""
}

$scriptPath = Join-Path -Path $PSScriptRoot -ChildPath "save_prompt_enhanced.ps1"

do {
    Show-Menu
    $choice = Read-Host "Enter choice"
    
    switch ($choice.ToUpper()) {
        '1' {
            $slug = Read-Host "Enter prompt slug (e.g., 'add-feature')"
            if (-not [string]::IsNullOrWhiteSpace($slug)) {
                & $scriptPath -Role user -Slug $slug -FromClipboard
                Write-Host "`nPress Enter to continue..." -ForegroundColor Gray
                Read-Host
            }
        }
        '2' {
            $slug = Read-Host "Enter response slug (e.g., 'feature-impl')"
            if (-not [string]::IsNullOrWhiteSpace($slug)) {
                & $scriptPath -Role assistant -Slug $slug -FromClipboard
                Write-Host "`nPress Enter to continue..." -ForegroundColor Gray
                Read-Host
            }
        }
        '3' {
            $slug = Read-Host "Enter prompt slug (e.g., 'add-feature')"
            if (-not [string]::IsNullOrWhiteSpace($slug)) {
                & $scriptPath -Role user -Slug $slug -Interactive
                Write-Host "`nPress Enter to continue..." -ForegroundColor Gray
                Read-Host
            }
        }
        '4' {
            Get-RecentLogs
            Write-Host "Press Enter to continue..." -ForegroundColor Gray
            Read-Host
        }
        'Q' {
            Write-Host "`nGoodbye!" -ForegroundColor Cyan
            break
        }
        default {
            Write-Host "Invalid choice. Please try again.`n" -ForegroundColor Red
        }
    }
    
    Clear-Host
    Write-Host @"
================================
 AI Session Auto-Logger
================================
"@ -ForegroundColor Cyan
    
} while ($choice.ToUpper() -ne 'Q')
