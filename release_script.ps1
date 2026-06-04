# release_script.ps1
param(
    [string]$Mode,
    [string]$CustomVersion
)
$AppName = "FileTreeGen"
$versionFile = "release.props"

function Get-Version {
    [xml]$xml = Get-Content $versionFile
    return $xml.Project.PropertyGroup.Version
}

function Set-Version($newVersion) {
    [xml]$xml = Get-Content $versionFile
    $xml.Project.PropertyGroup.Version = $newVersion
    $xml.Project.PropertyGroup.AssemblyVersion = "$newVersion.0"
    $xml.Project.PropertyGroup.FileVersion = "$newVersion.0"
    $xml.Project.PropertyGroup.InformationalVersion = $newVersion
    $xml.Save((Resolve-Path $versionFile))
}

function Increment-Version($type, $current) {
    $parts = $current.Split(".") | ForEach-Object { [int]$_ }

    switch ($type) {
        "major" { $parts[0]++; $parts[1]=0; $parts[2]=0 }
        "minor" { $parts[1]++; $parts[2]=0 }
        "patch" { $parts[2]++ }
    }

    return "$($parts[0]).$($parts[1]).$($parts[2])"
}

$current = Get-Version
Write-Host "Current version: $current"
if (-not $Mode) {
    $Mode = Read-Host "Choose bump (major / minor / patch / custom)"
}
if ($Mode -eq "custom") {
    if (-not $CustomVersion) {
        $CustomVersion = Read-Host "Enter version (e.g. 1.2.3)"
    }
    $newVersion = $CustomVersion
}
else {
    $newVersion = Increment-Version $Mode $current
}
Set-Version $newVersion

# Build outputs
$Targets = @(
    "win-x64",
    "win-x86"
)
# Publishing sequence
Write-Host "Publishing version: $newVersion"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$ReleaseRoot = Join-Path $Root "releases"
if (Test-Path $ReleaseRoot) {
    Remove-Item $ReleaseRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $ReleaseRoot | Out-Null
Set-Location "$Root/"

foreach ($t in $Targets) {
    $OutputName = "{0}-{1}-{2}" -f $AppName, $newVersion, $t
    Write-Host ""
    Write-Host "Publishing: $OutputName"
    Write-Host ""
    dotnet publish -c Release -r $t `
        --self-contained true `
        /p:PublishAot=true `
        /p:PublishTrimmed=true `
        /p:PublishSingleFile=true `
        /p:DebugType=None `
        /p:DebugSymbols=false `
        -o "$ReleaseRoot"
    # rename output exe
    $exeOld = Join-Path $ReleaseRoot "$AppName.exe"
    $exeNew = Join-Path $ReleaseRoot "$OutputName.exe"
    if (Test-Path $exeOld) {
        Rename-Item $exeOld $exeNew -Force
    }
}
Write-Host "Done!"