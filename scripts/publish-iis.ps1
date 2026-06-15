# Publishes LibraryMicroservices for IIS and prepares folder structure.
#
# Usage:
#   .\scripts\publish-iis.ps1
#   .\scripts\publish-iis.ps1 -PublishRoot "D:\sites\library"
#   .\scripts\publish-iis.ps1 -InstallIisSites   # requires elevated PowerShell + IIS
#
# Prerequisites:
#   - .NET 10 SDK
#   - ASP.NET Core Hosting Bundle installed on the target server
#   - For -InstallIisSites: IIS with WebAdministration module and run as Administrator

[CmdletBinding()]
param(
    [string]$PublishRoot = "C:\inetpub\library",
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [switch]$OverwriteSeedData,
    [switch]$InstallIisSites
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$services = @(
    @{
        Folder   = "authors"
        Project  = "src\Authors\Authors.Api\Authors.Api.csproj"
        SiteName = "Library.Authors"
        AppPool  = "LibraryAuthorsPool"
        Port     = 5101
    },
    @{
        Folder   = "books"
        Project  = "src\Books\Books.Api\Books.Api.csproj"
        SiteName = "Library.Books"
        AppPool  = "LibraryBooksPool"
        Port     = 5102
    },
    @{
        Folder   = "loans"
        Project  = "src\Loans\Loans.Api\Loans.Api.csproj"
        SiteName = "Library.Loans"
        AppPool  = "LibraryLoansPool"
        Port     = 5103
    },
    @{
        Folder   = "gateway"
        Project  = "src\ApiGateway\ApiGateway\ApiGateway.csproj"
        SiteName = "Library.Gateway"
        AppPool  = "LibraryGatewayPool"
        Port     = 80
    }
)

function Set-AspNetCoreProductionEnvironment {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PublishPath
    )

    $webConfigPath = Join-Path $PublishPath "web.config"
    if (-not (Test-Path $webConfigPath)) {
        Write-Warning "web.config not found in $PublishPath"
        return
    }

    [xml]$webConfig = Get-Content $webConfigPath
    $aspNetCore = $webConfig.SelectSingleNode("//aspNetCore")
    if ($null -eq $aspNetCore) {
        Write-Warning "aspNetCore section not found in $webConfigPath"
        return
    }

    $envVars = $aspNetCore.SelectSingleNode("environmentVariables")
    if ($null -eq $envVars) {
        $envVars = $webConfig.CreateElement("environmentVariables")
        $aspNetCore.AppendChild($envVars) | Out-Null
    }

    $existing = $envVars.SelectSingleNode("environmentVariable[@name='ASPNETCORE_ENVIRONMENT']")
    if ($existing) {
        $existing.SetAttribute("value", "Production")
    }
    else {
        $envVar = $webConfig.CreateElement("environmentVariable")
        $envVar.SetAttribute("name", "ASPNETCORE_ENVIRONMENT")
        $envVar.SetAttribute("value", "Production")
        $envVars.AppendChild($envVar) | Out-Null
    }

    $aspNetCore.SetAttribute("stdoutLogEnabled", "true")
    $aspNetCore.SetAttribute("stdoutLogFile", ".\logs\stdout")

    $webConfig.Save($webConfigPath)
}

function Grant-DataFolderAccess {
    param(
        [Parameter(Mandatory = $true)]
        [string]$DataPath,
        [Parameter(Mandatory = $true)]
        [string[]]$AppPoolNames
    )

    foreach ($poolName in $AppPoolNames) {
        $identity = "IIS AppPool\$poolName"
        $previousPreference = $ErrorActionPreference
        $ErrorActionPreference = "Continue"
        icacls $DataPath /grant "${identity}:(OI)(CI)M" /Q *> $null
        $exitCode = $LASTEXITCODE
        $ErrorActionPreference = $previousPreference

        if ($exitCode -eq 0) {
            Write-Host "  Granted Modify to $identity"
        }
        else {
            Write-Warning "Could not grant access to $identity (create the app pool in IIS first, then rerun or set permissions manually)."
        }
    }
}

function Copy-SeedDataFile {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourceFile,
        [Parameter(Mandatory = $true)]
        [string]$TargetFile,
        [switch]$Force
    )

    if ($Force -or -not (Test-Path $TargetFile)) {
        Copy-Item -Path $SourceFile -Destination $TargetFile -Force
        Write-Host "  Data: $(Split-Path $TargetFile -Leaf)"
    }
    else {
        Write-Host "  Data: $(Split-Path $TargetFile -Leaf) (kept existing)"
    }
}

Write-Host "Publishing LibraryMicroservices for IIS" -ForegroundColor Cyan
Write-Host "  Configuration : $Configuration"
Write-Host "  Publish root  : $PublishRoot"
Write-Host ""

New-Item -ItemType Directory -Path $PublishRoot -Force | Out-Null

foreach ($service in $services) {
    $outputPath = Join-Path $PublishRoot $service.Folder
    $projectPath = Join-Path $repoRoot $service.Project

    Write-Host "Publishing $($service.Folder)..." -ForegroundColor Yellow

    dotnet publish $projectPath `
        -c $Configuration `
        -o $outputPath `
        --no-self-contained `
        /p:UseAppHost=false

    New-Item -ItemType Directory -Path (Join-Path $outputPath "logs") -Force | Out-Null
    Set-AspNetCoreProductionEnvironment -PublishPath $outputPath

    Write-Host "  Output: $outputPath" -ForegroundColor Green
}

$dataPath = Join-Path $PublishRoot "data"
New-Item -ItemType Directory -Path $dataPath -Force | Out-Null

Write-Host ""
Write-Host "Preparing shared data folder..." -ForegroundColor Yellow

$seedFiles = @("authors.json", "books.json", "loans.json")
foreach ($seedFile in $seedFiles) {
    Copy-SeedDataFile `
        -SourceFile (Join-Path $repoRoot "data\$seedFile") `
        -TargetFile (Join-Path $dataPath $seedFile) `
        -Force:$OverwriteSeedData
}

Write-Host ""
Write-Host "Setting data folder permissions..." -ForegroundColor Yellow

$backendPools = @("LibraryAuthorsPool", "LibraryBooksPool", "LibraryLoansPool")
Grant-DataFolderAccess -DataPath $dataPath -AppPoolNames $backendPools

if ($InstallIisSites) {
    Write-Host ""
    Write-Host "Creating IIS app pools and sites..." -ForegroundColor Yellow

    Import-Module WebAdministration -ErrorAction Stop

    foreach ($service in $services) {
        $poolName = $service.AppPool
        $siteName = $service.SiteName
        $physicalPath = Join-Path $PublishRoot $service.Folder
        $port = $service.Port

        if (-not (Test-Path "IIS:\AppPools\$poolName")) {
            New-WebAppPool -Name $poolName | Out-Null
            Set-ItemProperty "IIS:\AppPools\$poolName" managedRuntimeVersion ""
            Write-Host "  Created app pool: $poolName"
        }

        if (Test-Path "IIS:\Sites\$siteName") {
            Stop-WebSite -Name $siteName -ErrorAction SilentlyContinue
            Remove-WebSite -Name $siteName
            Write-Host "  Recreated site: $siteName"
        }

        New-WebSite `
            -Name $siteName `
            -Port $port `
            -PhysicalPath $physicalPath `
            -ApplicationPool $poolName | Out-Null

        Write-Host "  Site: $siteName -> http://localhost:$port"
    }
}

Write-Host ""
Write-Host "Publish complete." -ForegroundColor Green
Write-Host ""
Write-Host "Folder layout:"
Write-Host "  $PublishRoot\authors\   (Authors.Api, IIS port 5101)"
Write-Host "  $PublishRoot\books\     (Books.Api,   IIS port 5102)"
Write-Host "  $PublishRoot\loans\     (Loans.Api,   IIS port 5103)"
Write-Host "  $PublishRoot\gateway\   (ApiGateway,  IIS port 80)"
Write-Host "  $PublishRoot\data\       (shared JSON files)"
Write-Host ""

if (-not $InstallIisSites) {
    Write-Host "Next steps (IIS Manager):" -ForegroundColor Cyan
    Write-Host "  1. Install the .NET 10 ASP.NET Core Hosting Bundle if not already installed."
    Write-Host "  2. Create 4 app pools (No Managed Code):"
    Write-Host "       LibraryAuthorsPool, LibraryBooksPool, LibraryLoansPool, LibraryGatewayPool"
    Write-Host "  3. Create 4 sites pointing to the folders above on ports 5101, 5102, 5103, and 80."
    Write-Host "  4. Start all sites and browse:"
    Write-Host "       http://localhost/health"
    Write-Host "       http://localhost/api/authors"
    Write-Host ""
    Write-Host "Or rerun as Administrator with -InstallIisSites to create pools and sites automatically."
}
else {
    Write-Host "IIS sites created. Test with:"
    Write-Host "  http://localhost/health"
    Write-Host "  http://localhost/api/authors"
}

Write-Host ""
Write-Host "Stdout logs (if needed): <publish-folder>\logs\" -ForegroundColor DarkGray
