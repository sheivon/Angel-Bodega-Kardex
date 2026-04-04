<#
.SYNOPSIS
    Pushes to origin/main, builds the solution, and runs the project.
.DESCRIPTION
    This script executes the commands in order: git push, dotnet build, then dotnet run.
    It stops if any step fails.
#>

param(
    [string]$Remote = 'origin',
    [string]$Branch = 'main',
    [string]$Solution = 'kardex-Web.sln',
    [string]$Project = 'kardex-Web.csproj',
    [string]$CommitMessage = 'PUSH'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Fail([string]$Message) {
    Write-Error $Message
    exit 1
}

Write-Host "=== Commit changes ===" -ForegroundColor Cyan
git add -A
$changes = git status --porcelain
if ($changes) {
    git commit -m $CommitMessage
    if ($LASTEXITCODE -ne 0) { Fail "Git commit failed with exit code $LASTEXITCODE." }
} else {
    Write-Host "No changes to commit." -ForegroundColor Yellow
}

Write-Host "=== Push to $Remote/$Branch ===" -ForegroundColor Cyan
git push $Remote $Branch
if ($LASTEXITCODE -ne 0) { Fail "Git push failed with exit code $LASTEXITCODE." }

Write-Host "=== Build solution $Solution ===" -ForegroundColor Cyan
dotnet build $Solution
if ($LASTEXITCODE -ne 0) { Fail "Build failed with exit code $LASTEXITCODE." }

Write-Host "=== Run project $Project ===" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop the running app..." -ForegroundColor Yellow
dotnet run --project $Project
if ($LASTEXITCODE -ne 0) { Fail "Run failed with exit code $LASTEXITCODE." }
