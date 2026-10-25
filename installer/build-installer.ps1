[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$InnoSetupPath = "",
    [switch]$SkipPublish,
    [switch]$SkipInstaller
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Runtime -ne "win-x64") {
    throw "El instalador solo admite el runtime win-x64."
}

if ($SkipPublish -and $SkipInstaller) {
    throw "No se puede omitir la publicación y la compilación del instalador al mismo tiempo."
}

$root = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $root "src/HistoriaClinica.App/HistoriaClinica.App.csproj"
$scriptPath = Join-Path $PSScriptRoot "HistoriaClinica.iss"
$publishDir = Join-Path $root "artifacts/publish/$Runtime"
$installerDir = Join-Path $root "artifacts/installer"

$versionOutput = & dotnet msbuild $projectPath -getProperty:Version -nologo
if ($LASTEXITCODE -ne 0) {
    throw "No se pudo leer la versión desde $projectPath."
}

$version = ($versionOutput | Select-Object -Last 1).Trim()
if ($version -notmatch "^\d+\.\d+\.\d+") {
    throw "Versión inválida en ${projectPath}: $version"
}

if (-not $SkipInstaller) {
    if (-not $InnoSetupPath) {
        $command = Get-Command "ISCC.exe" -ErrorAction SilentlyContinue
        if ($command) {
            $InnoSetupPath = $command.Source
        }
    }

    if (-not $InnoSetupPath) {
        $programFilesX86 = [Environment]::GetFolderPath("ProgramFilesX86")
        $programFiles = [Environment]::GetFolderPath("ProgramFiles")
        $localAppData = [Environment]::GetFolderPath("LocalApplicationData")
        $candidates = @(
            $programFilesX86,
            $programFiles,
            $localAppData
        ) | Where-Object { $_ } | ForEach-Object {
            Join-Path $_ "Inno Setup 6\ISCC.exe"
        }

        $InnoSetupPath = $candidates |
            Where-Object { Test-Path -LiteralPath $_ } |
            Select-Object -First 1
    }

    if (-not $InnoSetupPath -or -not (Test-Path -LiteralPath $InnoSetupPath)) {
        throw "No se encontró ISCC.exe de Inno Setup 6. Instalá Inno Setup o pasá -InnoSetupPath."
    }
}

if (-not $SkipPublish) {
    Remove-Item -LiteralPath $publishDir -Recurse -Force -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Path $publishDir -Force | Out-Null
}

if (-not $SkipInstaller) {
    New-Item -ItemType Directory -Path $installerDir -Force | Out-Null
}

Push-Location $root
try {
    if (-not $SkipPublish) {
        & dotnet publish $projectPath `
            -c $Configuration `
            -r $Runtime `
            --self-contained true `
            -p:PublishSingleFile=false `
            -p:DebugType=None `
            -p:DebugSymbols=false `
            -p:Version=$version `
            -o $publishDir

        if ($LASTEXITCODE -ne 0) {
            throw "Falló dotnet publish con código $LASTEXITCODE."
        }
    }
    elseif (-not (Test-Path -LiteralPath (Join-Path $publishDir "HistoriaClinica.exe"))) {
        throw "No existe la publicación en $publishDir. Ejecutá sin -SkipPublish primero."
    }

    if (-not $SkipInstaller) {
        & $InnoSetupPath "/DAppVersion=$version" $scriptPath
        if ($LASTEXITCODE -ne 0) {
            throw "Falló Inno Setup con código $LASTEXITCODE."
        }
    }
}
finally {
    Pop-Location
}

if ($SkipInstaller) {
    Write-Host "Publicación: $(Join-Path $publishDir 'HistoriaClinica.exe')"
    return
}

$installer = Join-Path $installerDir "HistoriaClinica-$version-setup.exe"
if (-not (Test-Path -LiteralPath $installer)) {
    throw "No se encontró el instalador generado en $installer."
}

$hash = (Get-FileHash -LiteralPath $installer -Algorithm SHA256).Hash.ToLowerInvariant()
$checksumPath = "$installer.sha256"
Set-Content -LiteralPath $checksumPath -Encoding ascii -Value "$hash  $(Split-Path -Leaf $installer)"

Write-Host "Instalador: $installer"
Write-Host "SHA256:    $checksumPath"
