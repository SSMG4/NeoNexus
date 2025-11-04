<#
Build script for Windows native + managed projects (dev script).
Enforces x64 builds and uses Release by default.
#>

param(
    [switch] $Publish = $false
)

$ErrorActionPreference = 'Stop'
$configuration = "Release"
$arch = "x64"

Write-Host "Building native project (x64, Release)..."
Push-Location Windows/native
if (-Not (Test-Path build)) { New-Item -ItemType Directory -Path build | Out-Null }
Push-Location build
cmake .. -G "Visual Studio 17 2022" -A x64
cmake --build . --config $configuration
Pop-Location
Pop-Location

Write-Host "Building managed WinDemo (Release, win-x64)..."
Push-Location Windows/win/NeoNexus.WinDemo
dotnet restore
dotnet build -c $configuration
if ($Publish) {
    dotnet publish -c $configuration -r win-x64 -o "..\..\publish\win-x64\"
}
Pop-Location

Write-Host "Build completed."
