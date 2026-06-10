$ErrorActionPreference = 'Stop'

$databaseProjectDirectory = $PSScriptRoot
$databaseProjectPath = Join-Path -Path $databaseProjectDirectory -ChildPath 'Template.Database.csproj'
$migrationName = 'AutoMigration_' + (Get-Date -Format 'yyyyMMdd_HHmmss')

if (-not (Test-Path -Path $databaseProjectPath)) {
	throw "Database project file was not found at '$databaseProjectPath'."
}

Push-Location $databaseProjectDirectory
try {
	Write-Host "Using database project: $databaseProjectPath"
	Write-Host "Using startup project: $databaseProjectPath"
	Write-Host "Generated migration name: $migrationName"

	dotnet ef migrations add $migrationName --project $databaseProjectPath
	if ($LASTEXITCODE -ne 0) {
		throw 'Failed to add the Entity Framework migration.'
	}

	dotnet ef database update --project $databaseProjectPath
	if ($LASTEXITCODE -ne 0) {
		throw 'Failed to update the database.'
	}

	Write-Host 'Entity Framework migration and database update completed successfully.'
}
finally {
	Pop-Location
}
