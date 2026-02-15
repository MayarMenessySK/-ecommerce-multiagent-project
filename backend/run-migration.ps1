# ECommerce Migration Runner
param(
    [string]$Password = "",
    [switch]$Up,
    [long]$Down = -1,
    [string]$Env = "Development"
)

Write-Host "`n🔄 E-Commerce Migration Runner`n" -ForegroundColor Cyan

# Get password if not provided
if (-not $Password) {
    $securePassword = Read-Host "Enter PostgreSQL password for 'postgres' user" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# Update appsettings.json with password
$appSettingsPath = "ECommerce.MigrationRunner\appsettings.json"
$json = Get-Content $appSettingsPath | ConvertFrom-Json
$json.ConnectionStrings.Development = "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=$Password"
$json | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath

Write-Host "📦 Building migration runner..." -ForegroundColor Yellow
cd ECommerce.MigrationRunner
dotnet build --configuration Release --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    cd ..
    exit 1
}

Write-Host "✅ Build successful`n" -ForegroundColor Green

# Run migrations
if ($Up) {
    dotnet run --no-build --configuration Release -- --up --env $Env
} elseif ($Down -ge 0) {
    dotnet run --no-build --configuration Release -- --down $Down --env $Env
} else {
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\run-migration.ps1 -Up                    # Migrate to latest" -ForegroundColor Gray
    Write-Host "  .\run-migration.ps1 -Down 1                # Rollback to version 1" -ForegroundColor Gray
    Write-Host "  .\run-migration.ps1 -Up -Env Production    # Use production DB" -ForegroundColor Gray
}

cd ..
