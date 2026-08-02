$ErrorActionPreference = 'Stop'

# Entity Framework Core - Code First Migration Script
# Handles both new and existing databases intelligently

$projectDirectory = $PSScriptRoot
$projectPath = Join-Path $projectDirectory 'Template.Database.csproj'
$contextName = 'TemplateDbContext'
$migrationsPath = Join-Path $projectDirectory 'Migrations'

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
    # Step 0: Check database state
    Write-Host "=================================================="
    Write-Host "Step 0: Checking Database State"
    Write-Host "=================================================="
    Write-Host ""

    # Try to get migration list
    $migrationList = @(dotnet ef migrations list `
        --project $projectPath `
        --context $contextName 2>&1)

    $hasInitialMigration = $migrationList | Select-String "Initial" -Quiet
    $hasMigrations = ($migrationList.Count -gt 0) -and (-not ($migrationList[0] -match "No migrations"))

    if (-not $hasMigrations) {
        Write-Host "[!] No migration history found" -ForegroundColor Yellow
        Write-Host "[!] This appears to be an existing database" -ForegroundColor Yellow
        Write-Host "[*] Creating Initial migration to track current schema..." -ForegroundColor Cyan
        Write-Host ""

        # Create Initial migration
        dotnet ef migrations add Initial `
            --project $projectPath `
            --context $contextName 2>&1 | Out-Null

        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] Initial migration created" -ForegroundColor Green

            # Find and edit the Initial migration to remove table creation
            $initialMigrationFile = Get-ChildItem -Path $migrationsPath -Filter "*.cs" `
                | Where-Object { $_.Name -match "Initial\.cs$" -and $_.Name -notmatch "Designer" } `
                | Select-Object -First 1

            if ($null -ne $initialMigrationFile) {
                Write-Host "[*] Emptying Initial migration (tables already exist)..." -ForegroundColor Cyan

                $content = Get-Content $initialMigrationFile.FullName -Raw

                # Use more robust regex that handles nested braces
                # Replace entire Up method body with empty
                $content = $content -replace `
                    '(protected override void Up\(MigrationBuilder migrationBuilder\)\s*\{).*?(\n\s*})', `
                    '${1}`n        `n    ${2}'

                # Replace entire Down method body with empty
                $content = $content -replace `
                    '(protected override void Down\(MigrationBuilder migrationBuilder\)\s*\{).*?(\n\s*})', `
                    '${1}`n        `n    ${2}'

                Set-Content -Path $initialMigrationFile.FullName -Value $content
                Write-Host "[OK] Initial migration emptied (no table recreation)" -ForegroundColor Green
            }

            # Apply the empty Initial migration
            Write-Host "[*] Applying Initial migration..." -ForegroundColor Cyan
            dotnet ef database update Initial `
                --project $projectPath `
                --context $contextName 2>&1

            if ($LASTEXITCODE -ne 0) {
                Write-Host "[ERROR] Could not apply Initial migration" -ForegroundColor Red
                exit 1
            }

            Write-Host "[OK] Initial migration applied" -ForegroundColor Green
        }
        else {
            Write-Host "[!] Initial migration creation reported issues" -ForegroundColor Yellow
        }
        Write-Host ""
    }

    # Step 1: Check for model changes
    Write-Host "=================================================="
    Write-Host "Step 1: Checking for Model Changes"
    Write-Host "=================================================="
    Write-Host ""

    Write-Host "[*] Analyzing model changes..." -ForegroundColor Cyan
    Write-Host "[*] Context: $contextName" -ForegroundColor Cyan
    Write-Host ""

    dotnet ef migrations has-pending-model-changes `
        --project $projectPath `
        --context $contextName 2>&1 | Out-Null

    if ($LASTEXITCODE -eq 0) {
        Write-Host "[!] No changes detected" -ForegroundColor Yellow
        Write-Host "[!] Your model is already in sync with the database." -ForegroundColor Yellow
        Write-Host ""
    }
    else {
        $timestamp = Get-Date -Format "yyyyMMddHHmmss"
        $migrationName = "Auto_$timestamp"

        Write-Host "[*] Generating migration: $migrationName" -ForegroundColor Cyan
        Write-Host ""

        dotnet ef migrations add $migrationName `
            --project $projectPath `
            --context $contextName 2>&1

        if ($LASTEXITCODE -ne 0) {
            Write-Host ""
            Write-Host "[ERROR] Migration generation failed" -ForegroundColor Red
            exit 1
        }

        Write-Host ""
        Write-Host "[OK] Migration generated: $migrationName" -ForegroundColor Green
        Write-Host ""

        # Step 3: Update Database
        Write-Host "=================================================="
        Write-Host "Step 3: Updating Database"
        Write-Host "=================================================="
        Write-Host ""

        Write-Host "[*] Applying migration to database..." -ForegroundColor Cyan
        Write-Host ""

        dotnet ef database update `
            --project $projectPath `
            --context $contextName 2>&1

        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERROR] Database update failed" -ForegroundColor Red
            Write-Host "[!] Check the error above for details" -ForegroundColor Yellow
            exit 1
        }

        Write-Host ""
        Write-Host "[OK] Database updated successfully!" -ForegroundColor Green
        Write-Host ""
    }

    # Step 4: List Migrations
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
