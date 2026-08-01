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
    # Step 0: Check if this is an existing database
    Write-Host "=================================================="
    Write-Host "Step 0: Checking Database State"
    Write-Host "=================================================="
    Write-Host ""

    # Try to list existing migrations
    $migrationList = dotnet ef migrations list `
        --project $projectPath `
        --context $contextName 2>&1

    $hasInitialMigration = $migrationList | Select-String "Initial" -Quiet

    if (-not $hasInitialMigration) {
        Write-Host "[!] No migration history found" -ForegroundColor Yellow
        Write-Host "[!] Assuming existing database - initializing migration history..." -ForegroundColor Yellow
        Write-Host ""

        # Create Initial migration that represents the current database state
        Write-Host "[*] Creating Initial migration..." -ForegroundColor Cyan
        dotnet ef migrations add Initial `
            --project $projectPath `
            --context $contextName

        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] Initial migration created" -ForegroundColor Green
            Write-Host "[!] Note: Initial migration won't recreate existing tables" -ForegroundColor Yellow
            Write-Host ""
        }
        else {
            Write-Host "[!] Could not create Initial migration (expected for existing databases)" -ForegroundColor Yellow
            Write-Host ""
        }
    }

    # Step 1: Generate Migration
    Write-Host "=================================================="
    Write-Host "Step 1: Checking for Model Changes"
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
        --context $contextName 2>&1

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
            --context $contextName 2>&1

        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERROR] Database update failed" -ForegroundColor Red
            Write-Host "[!] This may occur if tables already exist in the database." -ForegroundColor Yellow
            Write-Host "[!] The migration was created but not applied." -ForegroundColor Yellow
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
