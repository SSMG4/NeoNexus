<#
Simple packaging helper (copy native dll into managed publish).
Assumes native build output at Windows/native/build/bin/NeoNexusNative.dll (x64 build).
Assumes managed publish output in Windows/win/NeoNexus.WinDemo/bin/Release/net7.0-windows/win-x64/publish
Run: .\package.ps1 after building/publishing
#>

$ErrorActionPreference = 'Stop'
$managedPublish = "Windows/win/NeoNexus.WinDemo/bin/Release/net7.0-windows/win-x64/publish"
$nativeDll = "Windows/native/build/bin/NeoNexusNative.dll"
$artifactDir = "Windows/build/artifacts"
$msiOut = Join-Path $artifactDir "NeoNexus.msi"

if (-Not (Test-Path $managedPublish)) {
    Write-Host "Managed publish folder not found. Try dotnet publish first or run build-windows.ps1 -Publish."
    exit 1
}

if (-Not (Test-Path $artifactDir)) {
    New-Item -ItemType Directory -Path $artifactDir | Out-Null
}

if (Test-Path $nativeDll) {
    Copy-Item $nativeDll $managedPublish -Force
    Write-Host "Copied native DLL to managed publish folder."
} else {
    Write-Host "Native DLL not found at $nativeDll"
}

# Optional: invoke WiX to produce MSI if WiX is installed and Product.wxs is configured.
$wixProduct = "Windows/installer/wix/Product.wxs"
if (Test-Path $wixProduct) {
    Write-Host "Building MSI with WiX (if candle & light present)..."
    Push-Location Windows/installer/wix
    if (Get-Command candle.exe -ErrorAction SilentlyContinue) {
        candle.exe -arch x64 Product.wxs
        if (Get-Command light.exe -ErrorAction SilentlyContinue) {
            light.exe -ext WixUIExtension Product.wixobj -out $msiOut
            Write-Host "Created MSI at $msiOut"
            Copy-Item $msiOut $artifactDir -Force
        } else {
            Write-Host "WiX 'light.exe' not found in PATH. Skipping MSI creation."
        }
    } else {
        Write-Host "WiX 'candle.exe' not found in PATH. Skipping MSI creation."
    }
    Pop-Location
} else {
    Write-Host "WiX Product.wxs not found; skipping MSI creation."
}
