Param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$proj = Join-Path $PSScriptRoot "ScaffGuard.csproj"
$publishDir = Join-Path $PSScriptRoot "bin\$Configuration\net8.0-windows\$Runtime\publish"

Write-Host "Publishing self-contained..."
dotnet publish $proj -c $Configuration -r $Runtime --self-contained true /p:PublishSingleFile=true

# Verify Inno Setup compiler
$iscc = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
if (!(Test-Path $iscc)) {
    $iscc = "${env:ProgramFiles}\Inno Setup 6\ISCC.exe"
}
if (!(Test-Path $iscc)) {
    Write-Warning "Inno Setup 6 not found. Please install from https://jrsoftware.org/isinfo.php"
    Write-Host "You can still run the app from $publishDir\ScaffGuard.exe"
    exit 0
}

# Prepare installer workspace
$installerDir = Join-Path $PSScriptRoot "Installer"
$srcPublish = Join-Path $installerDir "publish"
if (Test-Path $srcPublish) { Remove-Item $srcPublish -Recurse -Force }
New-Item -ItemType Directory -Path $srcPublish | Out-Null

Copy-Item "$publishDir\*" $srcPublish -Recurse

Write-Host "Building installer with Inno Setup..."
& $iscc (Join-Path $installerDir "ScaffGuard.iss")

$setup = Join-Path $installerDir "Output\ScaffGuard-Setup.exe"
if (Test-Path $setup) {
    Write-Host "Installer built: $setup"
} else {
    Write-Error "Installer build failed."
}
