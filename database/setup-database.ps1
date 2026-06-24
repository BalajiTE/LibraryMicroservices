param(
    [string]$Server = "localhost",
    [string]$Database = "LibraryMicroservices",
    [switch]$AuthOnly
)

$ErrorActionPreference = "Stop"
$scriptRoot = $PSScriptRoot
$sqlRoot = Join-Path $scriptRoot "sql"

function Invoke-SqlScript {
    param(
        [string]$InputFile,
        [string]$TargetDatabase = "master"
    )

    Write-Host "Running $InputFile against $TargetDatabase on $Server..."
    sqlcmd -S $Server -E -d $TargetDatabase -b -i $InputFile
    if ($LASTEXITCODE -ne 0) {
        throw "sqlcmd failed for $InputFile (exit code $LASTEXITCODE)"
    }
}

if ($AuthOnly) {
    Invoke-SqlScript -InputFile (Join-Path $sqlRoot "01-create-database.sql")
    Invoke-SqlScript -InputFile (Join-Path $sqlRoot "04-add-auth-tables.sql") -TargetDatabase $Database
}
else {
    Invoke-SqlScript -InputFile (Join-Path $sqlRoot "01-create-database.sql")
    Invoke-SqlScript -InputFile (Join-Path $sqlRoot "02-create-tables.sql") -TargetDatabase $Database
    Invoke-SqlScript -InputFile (Join-Path $sqlRoot "03-seed-data.sql") -TargetDatabase $Database
    Invoke-SqlScript -InputFile (Join-Path $sqlRoot "04-add-auth-tables.sql") -TargetDatabase $Database
}

Write-Host ""
Write-Host "Database '$Database' is ready on $Server."
Write-Host "Connection string example:"
Write-Host "Server=$Server;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True"
