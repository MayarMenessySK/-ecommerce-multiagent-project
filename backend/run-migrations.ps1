# PostgreSQL Migration Runner
param(
    [string]$ConnectionString = "",
    [string]$Version = "",
    [string]$Password = ""
)

# Colors for output
$ErrorColor = "Red"
$SuccessColor = "Green"
$InfoColor = "Cyan"
$WarningColor = "Yellow"

Write-Host "`n🔄 E-Commerce Database Migration Runner`n" -ForegroundColor $InfoColor

# Get password if not provided
if (-not $Password -and -not $ConnectionString) {
    $securePassword = Read-Host "Enter PostgreSQL password for 'postgres' user" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# Build connection string if not provided
if (-not $ConnectionString) {
    $ConnectionString = "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=$Password"
}

Write-Host "📊 Configuration:" -ForegroundColor $InfoColor
Write-Host "  Database: ecommerce" -ForegroundColor Gray
Write-Host "  Host: localhost:5432" -ForegroundColor Gray
Write-Host "  User: postgres" -ForegroundColor Gray
if ($Version) {
    Write-Host "  Target Version: $Version" -ForegroundColor Gray
} else {
    Write-Host "  Target Version: Latest (all migrations)" -ForegroundColor Gray
}

Write-Host "`n📦 Building ECommerce.Data project..." -ForegroundColor $WarningColor

$projectPath = "D:\source\ecommerce-multiagent-project\backend\ECommerce.Data"
Set-Location $projectPath

dotnet build --configuration Release --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor $ErrorColor
    exit 1
}

Write-Host "✅ Build successful`n" -ForegroundColor $SuccessColor

# Check if assembly exists
$assemblyPath = "bin\Release\net10.0\ECommerce.Data.dll"
if (-not (Test-Path $assemblyPath)) {
    Write-Host "❌ Assembly not found: $assemblyPath" -ForegroundColor $ErrorColor
    exit 1
}

Write-Host "🚀 Applying migrations..." -ForegroundColor $WarningColor
Write-Host "   Assembly: $assemblyPath" -ForegroundColor Gray

# Run migrations
try {
    if ($Version) {
        Write-Host "   Migrating to version: $Version`n" -ForegroundColor Gray
        dotnet fm migrate -p postgres -c $ConnectionString -a $assemblyPath --version $Version
    } else {
        Write-Host "   Migrating to latest version`n" -ForegroundColor Gray
        dotnet fm migrate -p postgres -c $ConnectionString -a $assemblyPath
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✅ Migrations applied successfully!" -ForegroundColor $SuccessColor
        Write-Host "`n📋 Migration Summary:" -ForegroundColor $InfoColor
        Write-Host "  V1  - Initial Schema (10 tables)" -ForegroundColor Gray
        Write-Host "  V2  - Coupons (2 tables)" -ForegroundColor Gray
        Write-Host "  V3  - Wishlists (2 tables)" -ForegroundColor Gray
        Write-Host "  V4  - Payments (2 tables)" -ForegroundColor Gray
        Write-Host "  V5  - Views (7 views)" -ForegroundColor Gray
        Write-Host "  V100 - Seed Data" -ForegroundColor Gray
        Write-Host "`n  Total: 16 tables + 7 views`n" -ForegroundColor $SuccessColor
    } else {
        Write-Host "`n❌ Migration failed!" -ForegroundColor $ErrorColor
        exit 1
    }
} catch {
    Write-Host "`n❌ Migration error: $_" -ForegroundColor $ErrorColor
    exit 1
}

Write-Host "🎉 Database setup complete!`n" -ForegroundColor $SuccessColor
