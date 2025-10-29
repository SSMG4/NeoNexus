<#
Build script for Windows native + managed projects (dev script).
Requires: Visual Studio (msbuild/cmake), .NET SDK
#>

$ErrorActionPreference = 'Stop'

Write-Host "Building native project..."
Push-Location Windows/native
if (-Not (Test-Path build)) { New-Item -ItemType Directory -Path build | Out-Null }
Push-Location build
cmake .. -G "Visual Studio 17 2022" -A x64
cmake --build . --config Release
Pop-Location
Pop-Location

Write-Host "Building managed WinDemo..."
Push-Location Windows/win/NeoNexus.WinDemo
dotnet build
Pop-Location

Write-Host "Build completed."
