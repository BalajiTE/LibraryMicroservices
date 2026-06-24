# Starts all LibraryMicroservices APIs + the API Gateway.
# Usage: .\scripts\start-all.ps1

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

Write-Host "Starting LibraryMicroservices..." -ForegroundColor Cyan
Write-Host "  Authors.Api  -> http://localhost:5101"
Write-Host "  Books.Api    -> http://localhost:5102"
Write-Host "  Loans.Api    -> http://localhost:5103"
Write-Host "  Members.Api  -> http://localhost:5104"
Write-Host "  ApiGateway   -> http://localhost:5000"
Write-Host ""
Write-Host "Try: http://localhost:5000/api/members" -ForegroundColor Green
Write-Host "Press Ctrl+C in this window to stop all services." -ForegroundColor Yellow
Write-Host ""

$projects = @(
    @{ Name = "Authors.Api"; Path = "src\Authors\Authors.Api\Authors.Api.csproj"; Profile = "Authors.Api" },
    @{ Name = "Books.Api";   Path = "src\Books\Books.Api\Books.Api.csproj";       Profile = "Books.Api" },
    @{ Name = "Loans.Api";   Path = "src\Loans\Loans.Api\Loans.Api.csproj";       Profile = "Loans.Api" },
    @{ Name = "Members.Api"; Path = "src\Members\Members.Api\Members.Api.csproj"; Profile = "Members.Api" },
    @{ Name = "ApiGateway";  Path = "src\ApiGateway\ApiGateway\ApiGateway.csproj"; Profile = "ApiGateway" }
)

$jobs = @()
foreach ($project in $projects) {
    $fullPath = Join-Path $root $project.Path
    $jobs += Start-Job -Name $project.Name -ScriptBlock {
        param($Root, $ProjectPath, $Profile)
        Set-Location $Root
        dotnet run --project $ProjectPath --launch-profile $Profile
    } -ArgumentList $root, $fullPath, $project.Profile
    Start-Sleep -Seconds 2
}

try {
    while ($true) {
        Receive-Job -Job $jobs -ErrorAction SilentlyContinue | ForEach-Object { Write-Host $_ }
        Start-Sleep -Seconds 2
    }
}
finally {
    Write-Host "Stopping services..." -ForegroundColor Yellow
    $jobs | Stop-Job -Force
    $jobs | Remove-Job -Force
}
