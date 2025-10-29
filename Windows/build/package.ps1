<#
Simple packaging helper (copy native dll into managed output).
#>

$managedOut = "Windows/win/NeoNexus.WinDemo/bin/Debug/net7.0-windows"
$nativeDll = "Windows/native/build/bin/NeoNexusNative.dll"

if (-Not (Test-Path $managedOut)) {
    Write-Host "Managed output not found. Build first."
    exit 1
}

if (Test-Path $nativeDll) {
    Copy-Item $nativeDll $managedOut -Force
    Write-Host "Copied native DLL to managed output."
} else {
    Write-Host "Native DLL not found at $nativeDll"
}
