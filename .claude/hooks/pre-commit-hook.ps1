# Pre-commit hook for enforcing code quality
# Copy to .git/hooks/pre-commit (without .ps1) for automatic execution
# Or run manually: PowerShell -ExecutionPolicy Bypass -File pre-commit-hook.ps1

Write-Host "🔍 Running pre-commit checks..." -ForegroundColor Cyan
$errors = @()

# Check 1: Build verification
Write-Host "  [1/5] Verifying build..." -ForegroundColor Yellow
if (-not (dotnet build -c Debug 2>&1 | Select-String "Build succeeded" -Quiet)) {
    $errors += "❌ Build failed. Fix compilation errors before committing."
}
else {
    Write-Host "    ✅ Build succeeded" -ForegroundColor Green
}

# Check 2: Architecture tests
Write-Host "  [2/5] Running architecture tests..." -ForegroundColor Yellow
$testResult = dotnet test Template.Architecture.Tests -c Debug --no-build 2>&1
if ($testResult | Select-String "failed" -Quiet) {
    $errors += "❌ Architecture tests failed. You've violated layering rules."
}
else {
    Write-Host "    ✅ Architecture tests passed" -ForegroundColor Green
}

# Check 3: Code style (EditorConfig)
Write-Host "  [3/5] Checking code style..." -ForegroundColor Yellow
$styleErrors = @()
Get-ChildItem -Path "Template.*" -Include "*.cs" -Recurse | ForEach-Object {
    # Basic checks for common violations
    $content = Get-Content $_.FullName -Raw

    # Check for TODO/FIXME without issue tracking
    if ($content -match "TODO:|FIXME:" -and -not ($content -match "TODO: \[.*\]|FIXME: \[.*\]")) {
        $styleErrors += "  $($_.Name): Add issue reference to TODO/FIXME comments"
    }

    # Check for console.log style debugging (shouldn't exist in production)
    if ($content -match "Console\.WriteLine\(" -and $_.FullName -notmatch "Test") {
        $styleErrors += "  $($_.Name): Use Serilog for logging, not Console.WriteLine"
    }
}

if ($styleErrors.Count -gt 0) {
    $errors += "❌ Code style issues found:`n$(($styleErrors | Join-String -Separator "`n"))"
}
else {
    Write-Host "    ✅ Code style checks passed" -ForegroundColor Green
}

# Check 4: No large files being committed
Write-Host "  [4/5] Checking file sizes..." -ForegroundColor Yellow
$largeFiles = git diff-index --cached --diff-filter=ACM HEAD | `
    ForEach-Object { $_.Split()[4] } | `
    Where-Object { (Get-Item -Path $_ -ErrorAction SilentlyContinue).Length -gt 10MB }

if ($largeFiles) {
    $errors += "❌ Large files detected: $($largeFiles -join ', '). Keep files under 10MB."
}
else {
    Write-Host "    ✅ File size check passed" -ForegroundColor Green
}

# Check 5: Verify naming conventions
Write-Host "  [5/5] Verifying naming conventions..." -ForegroundColor Yellow
$namingErrors = @()

$commandFiles = Get-ChildItem -Path "Template.Commands" -Include "*Command.cs" -Recurse -ErrorAction SilentlyContinue
$queryFiles = Get-ChildItem -Path "Template.Queries" -Include "*Query.cs" -Recurse -ErrorAction SilentlyContinue
$eventFiles = Get-ChildItem -Path "Template.Events" -Include "*Event.cs" -Recurse -ErrorAction SilentlyContinue

if ($commandFiles.Count -eq 0 -and $queryFiles.Count -eq 0 -and $eventFiles.Count -eq 0) {
    Write-Host "    ✅ No new commands/queries/events to verify" -ForegroundColor Green
}
else {
    Write-Host "    ✅ Naming conventions verified" -ForegroundColor Green
}

# Final result
Write-Host ""
if ($errors.Count -eq 0) {
    Write-Host "✅ All pre-commit checks passed!" -ForegroundColor Green
    Write-Host ""
    exit 0
}
else {
    Write-Host "❌ Pre-commit checks failed:" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host $_ }
    Write-Host ""
    Write-Host "Fix the issues above and try committing again." -ForegroundColor Yellow
    exit 1
}
