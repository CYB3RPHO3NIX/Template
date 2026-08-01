#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for Microservice Generator CLI tool

.DESCRIPTION
    Builds the standalone C++ executable from source using CMake

.PARAMETER BuildType
    Build type: Release (default) or Debug

.PARAMETER Clean
    Clean build directory before building

.PARAMETER Help
    Show this help message

.EXAMPLE
    .\build.ps1                    # Release build
    .\build.ps1 -BuildType Debug   # Debug build
    .\build.ps1 -Clean             # Clean and rebuild
#>

param(
    [ValidateSet("Release", "Debug")]
    [string]$BuildType = "Release",

    [switch]$Clean,
    [switch]$Help
)

# Error handling
$ErrorActionPreference = "Stop"

# Colors
$colors = @{
    Reset   = "`e[0m"
    Bold    = "`e[1m"
    Green   = "`e[32m"
    Blue    = "`e[34m"
    Yellow  = "`e[33m"
    Red     = "`e[31m"
    Cyan    = "`e[36m"
}

function Write-Success { Write-Host "$($colors.Green)✓ $args$($colors.Reset)" }
function Write-Error-Custom { Write-Host "$($colors.Red)✗ $args$($colors.Reset)" -ForegroundColor Red }
function Write-Info { Write-Host "$($colors.Cyan)→ $args$($colors.Reset)" }
function Write-Warn { Write-Host "$($colors.Yellow)⚠ $args$($colors.Reset)" }

function Show-Help {
    Write-Host "Usage: .\build.ps1 [OPTIONS]`n"
    Write-Host "Options:"
    Write-Host "  -BuildType <type>    Build type: Release (default) or Debug"
    Write-Host "  -Clean               Clean build directory before building"
    Write-Host "  -Help                Show this help message`n"
    Write-Host "Examples:"
    Write-Host "  .\build.ps1                  # Release build"
    Write-Host "  .\build.ps1 -BuildType Debug # Debug build"
    Write-Host "  .\build.ps1 -Clean          # Clean and rebuild"
}

if ($Help) {
    Show-Help
    exit 0
}

Write-Host "$($colors.Bold)$($colors.Blue)"
Write-Host "╔════════════════════════════════════════════════════════════╗"
Write-Host "║   Microservice Generator - Build Script                   ║"
Write-Host "╚════════════════════════════════════════════════════════════╝"
Write-Host "$($colors.Reset)`n"

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$buildDir = Join-Path $scriptPath "build"

# Step 1: Check prerequisites
Write-Info "Checking prerequisites..."

try {
    $cmakeVersion = cmake --version 2>$null | Select-Object -First 1
    if ($LASTEXITCODE -ne 0) {
        throw "CMake not found"
    }
    Write-Success "CMake: $cmakeVersion"
}
catch {
    Write-Error-Custom "CMake not found. Please install CMake 3.16 or higher"
    Write-Host "  Download: https://cmake.org/download/"
    Write-Host "  Or install via: choco install cmake"
    exit 1
}

# Step 2: Clean if requested
if ($Clean) {
    if (Test-Path $buildDir) {
        Write-Warn "Cleaning build directory..."
        Remove-Item -Recurse -Force $buildDir
    }
}

# Step 3: Create build directory
if (-not (Test-Path $buildDir)) {
    New-Item -ItemType Directory -Path $buildDir -Force | Out-Null
}

# Step 4: Configure with CMake
Write-Info "Configuring CMake (BuildType: $BuildType)..."
Push-Location $buildDir

try {
    cmake -G "Visual Studio 17 2022" -DCMAKE_BUILD_TYPE=$BuildType (Join-Path $scriptPath) 2>&1

    if ($LASTEXITCODE -ne 0) {
        throw "CMake configuration failed"
    }
    Write-Success "CMake configuration successful"
}
catch {
    Write-Error-Custom "CMake configuration failed: $_"
    Pop-Location
    exit 1
}

# Step 5: Build
Write-Info "Building solution..."

try {
    cmake --build . --config $BuildType 2>&1

    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    Write-Success "Build successful"
}
catch {
    Write-Error-Custom "Build failed: $_"
    Pop-Location
    exit 1
}

Pop-Location

# Step 6: Verify binary
$binPath = Join-Path $buildDir "bin\$BuildType\microservice-gen.exe"
$altBinPath = Join-Path $buildDir "bin\microservice-gen.exe"

$actualBin = if (Test-Path $binPath) { $binPath } elseif (Test-Path $altBinPath) { $altBinPath } else { $null }

Write-Host "`n$($colors.Bold)$($colors.Green)"
Write-Host "╔════════════════════════════════════════════════════════════╗"
Write-Host "║   ✓ Build Completed Successfully!                         ║"
Write-Host "╚════════════════════════════════════════════════════════════╝"
Write-Host "$($colors.Reset)`n"

if ($actualBin) {
    Write-Success "Binary location: $actualBin"
    $size = (Get-Item $actualBin).Length / 1MB
    Write-Info "Binary size: $([Math]::Round($size, 2)) MB"
    Write-Info "`nTest the executable:"
    Write-Host "  $actualBin --help"
    Write-Host "  $actualBin --version"
    Write-Host "  $actualBin -n TestService -o .\output`n"
}
else {
    Write-Warn "Binary not found in expected location"
    Write-Host "  Check build directory: $buildDir`n"
}

Write-Success "Ready to use!"
