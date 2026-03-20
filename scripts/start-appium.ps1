param(
    [int]$Port = 4723,
    [string]$Address = "127.0.0.1",
    [string]$WinAppDriverPath = ""
)

$resolvedPath = $WinAppDriverPath

if ([string]::IsNullOrWhiteSpace($resolvedPath)) {
    $resolvedPath = Join-Path $PSScriptRoot "..\\WindowsApplicationDriver_1.2.1\\SourceDir\\Windows Application Driver\\WinAppDriver.exe"
}

$resolvedPath = (Resolve-Path $resolvedPath).Path
$env:APPIUM_WAD_PATH = $resolvedPath

Write-Host "Using APPIUM_WAD_PATH=$resolvedPath"
Write-Host "Starting Appium on $Address`:$Port"

appium --address $Address --port $Port
