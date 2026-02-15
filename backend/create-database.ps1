# Create E-Commerce Database
param(
    [string]$Password = ""
)

Write-Host "`n🗄️  E-Commerce Database Creator`n" -ForegroundColor Cyan

# Get password if not provided
if (-not $Password) {
    $securePassword = Read-Host "Enter PostgreSQL password for 'postgres' user" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

$psqlPath = "D:\Program Files\PostgreSQL\17\bin\psql.exe"

if (-not (Test-Path $psqlPath)) {
    Write-Host "❌ psql.exe not found at: $psqlPath" -ForegroundColor Red
    Write-Host "Please verify your PostgreSQL installation path" -ForegroundColor Yellow
    exit 1
}

Write-Host "📊 Creating database 'ecommerce'..." -ForegroundColor Yellow

# Set password environment variable
$env:PGPASSWORD = $Password

# Check if database already exists
$checkDb = & $psqlPath -h localhost -p 5432 -U postgres -d postgres -t -c "SELECT 1 FROM pg_database WHERE datname='ecommerce';" 2>&1

if ($checkDb -match "1") {
    Write-Host "⚠️  Database 'ecommerce' already exists!" -ForegroundColor Yellow
    $response = Read-Host "Do you want to drop and recreate it? (yes/no)"
    
    if ($response -eq "yes") {
        Write-Host "🗑️  Dropping existing database..." -ForegroundColor Yellow
        & $psqlPath -h localhost -p 5432 -U postgres -d postgres -c "DROP DATABASE ecommerce;" 2>&1
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Failed to drop database" -ForegroundColor Red
            $env:PGPASSWORD = $null
            exit 1
        }
    } else {
        Write-Host "✅ Using existing database" -ForegroundColor Green
        $env:PGPASSWORD = $null
        exit 0
    }
}

# Create database
Write-Host "📦 Creating database..." -ForegroundColor Yellow
& $psqlPath -h localhost -p 5432 -U postgres -d postgres -c "CREATE DATABASE ecommerce WITH ENCODING='UTF8' LC_COLLATE='English_United States.1252' LC_CTYPE='English_United States.1252' TEMPLATE=template0;" 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to create database" -ForegroundColor Red
    $env:PGPASSWORD = $null
    exit 1
}

Write-Host "✅ Database created successfully!" -ForegroundColor Green

# Grant permissions
Write-Host "🔐 Setting permissions..." -ForegroundColor Yellow
& $psqlPath -h localhost -p 5432 -U postgres -d postgres -c "GRANT ALL PRIVILEGES ON DATABASE ecommerce TO postgres;" 2>&1

# Verify
Write-Host "`n📋 Verifying database..." -ForegroundColor Cyan
$result = & $psqlPath -h localhost -p 5432 -U postgres -d ecommerce -c "SELECT version();" 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Database is ready!" -ForegroundColor Green
    Write-Host "`n📊 Connection Details:" -ForegroundColor Cyan
    Write-Host "  Host: localhost" -ForegroundColor Gray
    Write-Host "  Port: 5432" -ForegroundColor Gray
    Write-Host "  Database: ecommerce" -ForegroundColor Gray
    Write-Host "  Username: postgres" -ForegroundColor Gray
    Write-Host "`n🎯 Next Steps:" -ForegroundColor Cyan
    Write-Host "  1. Run migrations: .\run-migrations.ps1" -ForegroundColor Yellow
    Write-Host "  2. Test connection: .\test-db-connection.ps1" -ForegroundColor Yellow
    Write-Host "  3. Generate LLBLGen entities (manual step)`n" -ForegroundColor Yellow
} else {
    Write-Host "❌ Database verification failed" -ForegroundColor Red
}

# Clear password
$env:PGPASSWORD = $null
