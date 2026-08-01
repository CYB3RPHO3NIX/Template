#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Creates a new microservice from the Enterprise CQRS template.

.DESCRIPTION
    Clones and customizes the CQRS microservice template with your service name,
    replacing all "Template" references with your service name throughout the project.
    Works for both local filesystem and Git repository templates.

.PARAMETER ServiceName
    Name of the new microservice (e.g., InvoiceService, OrderService).
    Must be PascalCase. Required.

.PARAMETER OutputPath
    Directory where the service will be created. If not specified, uses current directory.
    Default: Current directory

.PARAMETER CompanyName
    Optional company name to replace "Enterprise" namespace prefix.
    Default: Enterprise

.PARAMETER TemplatePath
    Path to the template repository (local or Git URL).
    Default: Current directory (assumes running from template repo)

.PARAMETER DontOpen
    If specified, doesn't open the solution in Visual Studio after creation.

.PARAMETER SkipRestore
    If specified, skips NuGet restore after project creation.

.EXAMPLE
    .\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService"
    Creates InvoiceService in current directory

.EXAMPLE
    .\Create-MicroserviceFromTemplate.ps1 -ServiceName "OrderService" -OutputPath "C:\Projects" -CompanyName "Acme"
    Creates OrderService in C:\Projects with Acme as company namespace

.EXAMPLE
    .\Create-MicroserviceFromTemplate.ps1 -ServiceName "PaymentService" -TemplatePath "https://github.com/org/CQRSTemplate.git"
    Creates PaymentService from remote Git template

#>

param(
    [Parameter(Mandatory = $true, HelpMessage = "Service name (e.g., InvoiceService)")]
    [ValidatePattern('^[A-Z][a-zA-Z0-9]*$')]
    [string]$ServiceName,

    [Parameter(Mandatory = $false)]
    [string]$OutputPath = (Get-Location),

    [Parameter(Mandatory = $false)]
    [string]$CompanyName = "Enterprise",

    [Parameter(Mandatory = $false)]
    [string]$TemplatePath = (Get-Location),

    [Parameter(Mandatory = $false)]
    [switch]$DontOpen,

    [Parameter(Mandatory = $false)]
    [switch]$SkipRestore
)

# Strict error handling
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# Color output
function Write-Success { Write-Host "✓ $args" -ForegroundColor Green }
function Write-Error-Custom { Write-Host "✗ $args" -ForegroundColor Red }
function Write-Info { Write-Host "• $args" -ForegroundColor Cyan }
function Write-Warn { Write-Host "⚠ $args" -ForegroundColor Yellow }

function Test-IsValidServiceName {
    param([string]$Name)

    if ($Name -match '^[A-Z][a-zA-Z0-9]*$') {
        return $true
    }

    Write-Error-Custom "Invalid service name: '$Name'"
    Write-Info "Service name must be PascalCase and alphanumeric (e.g., InvoiceService)"
    return $false
}

function Test-PathExists {
    param([string]$Path, [string]$Description)

    if (Test-Path $Path) {
        return $true
    }

    Write-Error-Custom "$Description not found: $Path"
    return $false
}

function Get-TemplatePath {
    param([string]$TemplatePath)

    Write-Info "Resolving template path..."

    # Check if it's a Git URL
    if ($TemplatePath -match '^https?://.*\.git$') {
        Write-Info "Template is a Git repository"
        return $TemplatePath
    }

    # Check if it's a local path
    $localPath = Resolve-Path $TemplatePath -ErrorAction SilentlyContinue
    if ($null -ne $localPath) {
        if (Test-Path "$localPath\.git") {
            Write-Success "Template repository found: $localPath"
            return $localPath
        }
        elseif (Test-Path "$localPath\Template.sln") {
            Write-Success "Template directory found: $localPath"
            return $localPath
        }
    }

    Write-Error-Custom "Template not found at: $TemplatePath"
    Write-Info "Please provide a valid local path or Git repository URL"
    return $null
}

function Copy-TemplateProject {
    param(
        [string]$TemplatePath,
        [string]$ServiceName,
        [string]$OutputPath
    )

    Write-Info "Copying template project..."

    $destinationPath = Join-Path $OutputPath $ServiceName

    if (Test-Path $destinationPath) {
        Write-Error-Custom "Destination already exists: $destinationPath"
        return $null
    }

    try {
        # Clone if it's a Git URL
        if ($TemplatePath -match '^https?://') {
            Write-Info "Cloning from Git repository..."
            git clone --depth 1 $TemplatePath $destinationPath
            if ($LASTEXITCODE -ne 0) {
                throw "Git clone failed"
            }
            Remove-Item -Path (Join-Path $destinationPath ".git") -Recurse -Force
        }
        else {
            # Copy local files
            Write-Info "Copying local template files..."
            Copy-Item -Path "$TemplatePath\*" -Destination $destinationPath -Recurse -Force `
                -Exclude @('.git', '.gitignore', 'bin', 'obj', '.vs', '.vscode', '*.user', '*.log')
        }

        Write-Success "Template copied to: $destinationPath"
        return $destinationPath
    }
    catch {
        Write-Error-Custom "Failed to copy template: $_"
        return $null
    }
}

function Rename-ProjectFiles {
    param(
        [string]$ProjectPath,
        [string]$ServiceName
    )

    Write-Info "Renaming project files and folders..."

    $filesToRename = @()

    # Find all items containing "Template" in the name
    Get-ChildItem -Path $ProjectPath -Recurse -Name | ForEach-Object {
        if ($_ -like '*Template*') {
            $filesToRename += $_
        }
    }

    foreach ($item in $filesToRename) {
        $oldPath = Join-Path $ProjectPath $item
        $newName = $item -replace 'Template', $ServiceName
        $newPath = Join-Path $ProjectPath $newName

        if (Test-Path $oldPath) {
            Rename-Item -Path $oldPath -NewName $newName -ErrorAction SilentlyContinue
            Write-Info "Renamed: $item → $newName"
        }
    }

    Write-Success "Project files renamed"
}

function Replace-TextInFiles {
    param(
        [string]$ProjectPath,
        [string]$ServiceName,
        [string]$CompanyName
    )

    Write-Info "Replacing text in project files..."

    # File patterns to search in
    $filePatterns = @('*.cs', '*.csproj', '*.sln', '*.json', '*.md', '*.xml')

    # Get all files matching patterns
    $files = Get-ChildItem -Path $ProjectPath -Include $filePatterns -Recurse -File

    $replacementCount = 0

    foreach ($file in $files) {
        try {
            $content = Get-Content $file.FullName -Raw -Encoding UTF8
            $originalContent = $content

            # Replace Template with ServiceName
            $content = $content -replace '\bTemplate\b', $ServiceName
            $content = $content -replace '\btemplate\b', ($ServiceName.ToLower())

            # Replace Enterprise with CompanyName
            if ($CompanyName -ne "Enterprise") {
                $content = $content -replace '\bEnterprise\b', $CompanyName
                $content = $content -replace '\benterprise\b', ($CompanyName.ToLower())
            }

            if ($content -ne $originalContent) {
                Set-Content $file.FullName -Value $content -Encoding UTF8
                $replacementCount++
                Write-Info "Updated: $($file.Name)"
            }
        }
        catch {
            Write-Warn "Failed to update $($file.FullName): $_"
        }
    }

    Write-Success "Text replacement completed ($replacementCount files updated)"
}

function Update-SolutionFile {
    param(
        [string]$ProjectPath,
        [string]$ServiceName
    )

    Write-Info "Verifying solution file..."

    $slnFile = Get-ChildItem -Path $ProjectPath -Name "*.sln" | Select-Object -First 1

    if ($null -eq $slnFile) {
        Write-Warn "No .sln file found. Creating reference: $ServiceName.sln"
        return
    }

    Write-Success "Solution file found: $slnFile"
}

function Build-Solution {
    param([string]$ProjectPath)

    Write-Info "Building solution..."

    try {
        Push-Location $ProjectPath
        dotnet build

        if ($LASTEXITCODE -eq 0) {
            Write-Success "Build completed successfully"
        }
        else {
            Write-Warn "Build completed with warnings or errors"
        }
    }
    catch {
        Write-Error-Custom "Build failed: $_"
    }
    finally {
        Pop-Location
    }
}

function Restore-NugetPackages {
    param([string]$ProjectPath)

    if ($SkipRestore) {
        Write-Info "Skipping NuGet restore"
        return
    }

    Write-Info "Restoring NuGet packages..."

    try {
        Push-Location $ProjectPath
        dotnet restore

        if ($LASTEXITCODE -eq 0) {
            Write-Success "NuGet packages restored successfully"
        }
        else {
            Write-Warn "NuGet restore completed with warnings"
        }
    }
    catch {
        Write-Error-Custom "NuGet restore failed: $_"
    }
    finally {
        Pop-Location
    }
}

function Open-InVisualStudio {
    param([string]$ProjectPath)

    if ($DontOpen) {
        Write-Info "Skipping Visual Studio launch"
        return
    }

    Write-Info "Opening in Visual Studio..."

    $slnFile = Get-ChildItem -Path $ProjectPath -Name "*.sln" | Select-Object -First 1

    if ($null -ne $slnFile) {
        $slnPath = Join-Path $ProjectPath $slnFile

        try {
            # Try to open with Visual Studio 2022
            Start-Process -FilePath "devenv.exe" -ArgumentList "`"$slnPath`"" -ErrorAction SilentlyContinue
            Write-Success "Opened in Visual Studio"
        }
        catch {
            Write-Warn "Could not open in Visual Studio. Please open manually: $slnPath"
        }
    }
}

function Main {
    Write-Host ""
    Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║   Enterprise CQRS Microservice Generator                ║" -ForegroundColor Cyan
    Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""

    # Validation
    if (-not (Test-IsValidServiceName $ServiceName)) {
        exit 1
    }

    Write-Success "Service name: $ServiceName"
    Write-Success "Company name: $CompanyName"
    Write-Success "Output path: $OutputPath"

    # Resolve template
    $resolvedTemplatePath = Get-TemplatePath $TemplatePath
    if ($null -eq $resolvedTemplatePath) {
        exit 1
    }

    # Copy template
    $projectPath = Copy-TemplateProject $resolvedTemplatePath $ServiceName $OutputPath
    if ($null -eq $projectPath) {
        exit 1
    }

    # Customize
    Rename-ProjectFiles $projectPath $ServiceName
    Replace-TextInFiles $projectPath $ServiceName $CompanyName
    Update-SolutionFile $projectPath $ServiceName

    # Post-creation steps
    Restore-NugetPackages $projectPath
    Open-InVisualStudio $projectPath

    Write-Host ""
    Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Green
    Write-Host "║   ✓ Microservice created successfully!                  ║" -ForegroundColor Green
    Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  cd $($projectPath)"
    Write-Host "  dotnet build"
    Write-Host "  dotnet run --project $ServiceName.API"
    Write-Host ""
}

Main
