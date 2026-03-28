param(
    [string]$AppiumUrl = "http://127.0.0.1:4723/status",
    [string]$WinAppDriverUrl = "http://127.0.0.1:4724/wd/hub/status",
    [int]$WinAppDriverPort = 4724
)

$ProgressPreference = "SilentlyContinue"

function Show-Section {
    param([string]$Title)
    Write-Host ""
    Write-Host "=== $Title ==="
}

function Invoke-StatusRequest {
    param([string]$Url)

    try {
        $response = Invoke-WebRequest -UseBasicParsing -Uri $Url -TimeoutSec 5
        [pscustomobject]@{
            Url = $Url
            StatusCode = [int]$response.StatusCode
            Body = $response.Content
        }
    }
    catch {
        $statusCode = $null
        $body = $_.Exception.Message

        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode

            try {
                $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
                $body = $reader.ReadToEnd()
            }
            catch {
                $body = $_.Exception.Message
            }
        }

        [pscustomobject]@{
            Url = $Url
            StatusCode = $statusCode
            Body = $body
        }
    }
}

function Get-CurrentProcessElevation {
    $principal = [Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Get-HttpSysRegistration {
    param([int]$Port)

    $serviceState = netsh http show servicestate

    if ($LASTEXITCODE -ne 0) {
        return $null
    }

    $portPattern = "HTTP://127.0.0.1:${Port}:"
    $matchIndexes = @()

    for ($i = 0; $i -lt $serviceState.Count; $i++) {
        if ($serviceState[$i] -like "*$portPattern*") {
            $matchIndexes += $i
        }
    }

    if ($matchIndexes.Count -eq 0) {
        return $null
    }

    $results = New-Object System.Collections.Generic.List[object]
    $seenKeys = New-Object System.Collections.Generic.HashSet[string]

    foreach ($matchIndex in $matchIndexes) {
        $start = [Math]::Max(0, $matchIndex - 12)
        $end = [Math]::Min($serviceState.Count - 1, $matchIndex + 12)

        for ($i = $start; $i -le $end; $i++) {
            if ($serviceState[$i] -match 'Id\.\:\s*(\d+),\s*imagen:\s*(.+)$') {
                $processId = [int]$matches[1]
                $image = $matches[2].Trim()
                $key = "$processId|$image"

                if ($seenKeys.Add($key)) {
                    $results.Add([pscustomobject]@{
                        ProcessId = $processId
                        Image = $image
                    })
                }
            }
        }
    }

    return $results
}

Show-Section "Appium"
$appiumStatus = Invoke-StatusRequest -Url $AppiumUrl
$appiumStatus | Format-List

Show-Section "Host"
$operatingSystem = Get-CimInstance Win32_OperatingSystem
[pscustomobject]@{
    Caption = $operatingSystem.Caption
    Version = $operatingSystem.Version
    BuildNumber = $operatingSystem.BuildNumber
    IsElevated = Get-CurrentProcessElevation
} | Format-List

Show-Section "WinAppDriver Port Ownership"
$connection = Get-NetTCPConnection -LocalPort $WinAppDriverPort -State Listen -ErrorAction SilentlyContinue |
    Select-Object -First 1

if ($null -eq $connection) {
    Write-Host "Nothing is listening on port $WinAppDriverPort."
}
else {
    $process = Get-Process -Id $connection.OwningProcess -ErrorAction SilentlyContinue
    [pscustomobject]@{
        LocalAddress = $connection.LocalAddress
        LocalPort = $connection.LocalPort
        OwningProcess = $connection.OwningProcess
        ProcessName = $process.ProcessName
        ProcessPath = $process.Path
    } | Format-List
}

Show-Section "HTTP.sys Backing Process"
$httpSysRegistration = Get-HttpSysRegistration -Port $WinAppDriverPort

if ($null -eq $httpSysRegistration -or $httpSysRegistration.Count -eq 0) {
    Write-Host "No HTTP.sys registration was found for port $WinAppDriverPort."
}
else {
    $httpSysRegistration | ForEach-Object {
        $backingProcess = $null

        if ($_.ProcessId) {
            $backingProcess = Get-Process -Id $_.ProcessId -ErrorAction SilentlyContinue
        }

        [pscustomobject]@{
            ProcessId = $_.ProcessId
            Image = $_.Image
            ProcessName = $backingProcess.ProcessName
            StartTime = $backingProcess.StartTime
        }
    } | Format-List
}

Show-Section "WinAppDriver Process"
$winAppDriverProcess = Get-Process WinAppDriver -ErrorAction SilentlyContinue

if ($null -eq $winAppDriverProcess) {
    Write-Host "WinAppDriver.exe is not running."
}
else {
    $winAppDriverProcess | Select-Object Id, ProcessName, StartTime, Path | Format-List
}

Show-Section "WinAppDriver Status"
$wadStatus = Invoke-StatusRequest -Url $WinAppDriverUrl
$wadStatus | Format-List

Show-Section "Interpretation"
if ($connection -and $connection.OwningProcess -eq 4) {
    Write-Host "Port $WinAppDriverPort is exposed through HTTP.sys, so PID 4 alone is not enough to identify the real server."
}

if ($httpSysRegistration -and $httpSysRegistration.Count -gt 0) {
    $processIds = ($httpSysRegistration | ForEach-Object { $_.ProcessId }) -join ", "
    Write-Host "HTTP.sys on port $WinAppDriverPort is associated with PID(s): $processIds."
}

if (-not (Get-CurrentProcessElevation)) {
    Write-Host "The current PowerShell session is not elevated."
}

if ($operatingSystem.Caption -like "*Windows 11*") {
    Write-Host "The host is Windows 11. Appium Windows Driver documents Windows 10 as the supported host."
}

if ($winAppDriverProcess -and -not (Get-CurrentProcessElevation)) {
    Write-Host "If WinAppDriver was launched from this same unelevated session, retry from an Administrator PowerShell window."
}

if ($wadStatus.StatusCode -ne 200) {
    Write-Host "WinAppDriver is not responding with HTTP 200 on $WinAppDriverUrl."
}

if ($appiumStatus.StatusCode -eq 200 -and $wadStatus.StatusCode -ne 200) {
    Write-Host "Appium is reachable, but the Windows backend is not healthy."
}
