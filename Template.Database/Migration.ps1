$ErrorActionPreference = 'Stop'

# Entity Framework Core - Code First Migration Script
# This script manages database migrations using the Code First approach.

$projectDirectory = $PSScriptRoot
$projectPath = Join-Path $projectDirectory 'Template.Database.csproj'
$contextName = 'TemplateDbContext'

Write-Host ""
Write-Host "=================================================="
Write-Host "Entity Framework Core - Code First Approach"
Write-Host "=================================================="
Write-Host ""

# Validation
if (-not (Test-Path $projectPath)) {
    Write-Host "ERROR: Project file not found at: $projectPath" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Project file found" -ForegroundColor Green
Write-Host ""

# Check for dotnet ef CLI
$efVersion = dotnet ef --version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: dotnet ef CLI not found" -ForegroundColor Red
    Write-Host "Install with: dotnet tool install -g dotnet-ef" -ForegroundColor Yellow
    exit 1
}
Write-Host "[OK] EF Core CLI: $efVersion" -ForegroundColor Green
Write-Host ""

# Navigate to project directory
Push-Location $projectDirectory

try {
    # Step 1: Generate Migration
    Write-Host "=================================================="
    Write-Host "Step 1: Generating Migration"
    Write-Host "=================================================="
    Write-Host ""

    Write-Host "[*] Analyzing model changes..." -ForegroundColor Cyan
    Write-Host "[*] Context: $contextName" -ForegroundColor Cyan
    Write-Host ""

    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    $migrationName = "Auto_$timestamp"

    Write-Host "[*] Generating migration: $migrationName" -ForegroundColor Cyan
    Write-Host ""

    dotnet ef migrations add $migrationName `
        --project $projectPath `
        --context $contextName

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "[!] No changes detected" -ForegroundColor Yellow
        Write-Host "[!] Your model is already in sync with the database." -ForegroundColor Yellow
        Write-Host ""
    }
    else {
        Write-Host ""
        Write-Host "[OK] Migration generated: $migrationName" -ForegroundColor Green
        Write-Host ""

        # Step 2: Update Database
        Write-Host "=================================================="
        Write-Host "Step 2: Updating Database"
        Write-Host "=================================================="
        Write-Host ""

        Write-Host "[*] Applying migration to database..." -ForegroundColor Cyan
        Write-Host ""

        dotnet ef database update `
            --project $projectPath `
            --context $contextName

        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERROR] Database update failed" -ForegroundColor Red
            exit 1
        }

        Write-Host ""
        Write-Host "[OK] Database updated successfully!" -ForegroundColor Green
        Write-Host ""
    }

    # Step 3: List Migrations
    Write-Host "=================================================="
    Write-Host "Migration History"
    Write-Host "=================================================="
    Write-Host ""

    dotnet ef migrations list `
        --project $projectPath `
        --context $contextName

    Write-Host ""
    Write-Host "[OK] Code First migration completed!" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host "[ERROR] $_" -ForegroundColor Red
    exit 1
}
finally {
    Pop-Location
}

# Display helpful commands
Write-Host "=================================================="
Write-Host "Common Commands"
Write-Host "=================================================="
Write-Host ""
Write-Host "View migration list:" -ForegroundColor Yellow
Write-Host "  dotnet ef migrations list -p Template.Database.csproj" -ForegroundColor Gray
Write-Host ""
Write-Host "Revert to specific migration:" -ForegroundColor Yellow
Write-Host "  dotnet ef database update MigrationName -p Template.Database.csproj" -ForegroundColor Gray
Write-Host ""
Write-Host "Generate SQL script:" -ForegroundColor Yellow
Write-Host "  dotnet ef migrations script -p Template.Database.csproj -o migration.sql" -ForegroundColor Gray
Write-Host ""
