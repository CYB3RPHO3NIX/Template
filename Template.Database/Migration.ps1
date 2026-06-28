$ErrorActionPreference = 'Stop'

# -----------------------------------------------------------------------------
# Configuration
# -----------------------------------------------------------------------------

$projectDirectory = $PSScriptRoot
$projectPath = Join-Path $projectDirectory 'Template.Database.csproj'

$connectionString = 'Server=localhost;Database=TemplateDb;Trusted_Connection=True;TrustServerCertificate=True'
$provider = 'Microsoft.EntityFrameworkCore.SqlServer'

$contextName = 'TemplateDbContext'
$contextDirectory = 'Domain\Contexts'
$entityDirectory = 'Domain\Entities'

# -----------------------------------------------------------------------------
# Validation
# -----------------------------------------------------------------------------

if (-not (Test-Path $projectPath)) {
    throw "Project file was not found at '$projectPath'."
}

# -----------------------------------------------------------------------------
# Scaffold
# -----------------------------------------------------------------------------

Push-Location $projectDirectory

try {
    Write-Host ''
    Write-Host '==================================================='
    Write-Host 'Entity Framework Database First Scaffolding'
    Write-Host '==================================================='
    Write-Host "Project: $projectPath"
    Write-Host "Context: $contextName"
    Write-Host "Context Directory: $contextDirectory"
    Write-Host "Entity Directory: $entityDirectory"
    Write-Host ''

    dotnet ef dbcontext scaffold `
        "$connectionString" `
        $provider `
        --project $projectPath `
        --context $contextName `
        --context-dir $contextDirectory `
        --output-dir $entityDirectory `
        --force `
        --no-onconfiguring

    if ($LASTEXITCODE -ne 0) {
        throw 'Database scaffolding failed.'
    }

    Write-Host ''
    Write-Host 'Database scaffolding completed successfully.'
    Write-Host ''
}
finally {
    Pop-Location
}