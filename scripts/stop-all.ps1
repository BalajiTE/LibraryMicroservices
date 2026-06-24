# Stops all LibraryMicroservices API and gateway processes.
# Usage: .\scripts\stop-all.ps1

$names = @("ApiGateway", "Authors.Api", "Books.Api", "Loans.Api", "Members.Api", "Auth.Api")
$stopped = 0

foreach ($name in $names) {
    Get-Process -Name $name -ErrorAction SilentlyContinue | ForEach-Object {
        Stop-Process -Id $_.Id -Force
        Write-Host "Stopped $name (PID $($_.Id))"
        $stopped++
    }
}

Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" -ErrorAction SilentlyContinue |
    Where-Object { $_.CommandLine -like '*LibraryMicroservices*' } |
    ForEach-Object {
        Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue
        Write-Host "Stopped dotnet (PID $($_.ProcessId))"
        $stopped++
    }

if ($stopped -eq 0) {
    Write-Host "No running LibraryMicroservices processes found."
} else {
    Write-Host "Done. You can rebuild now." -ForegroundColor Green
}
