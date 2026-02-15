# Test PostgreSQL Database Connection
param(
    [string]$Host = "localhost",
    [string]$Port = "5432",
    [string]$Database = "ecommerce",
    [string]$Username = "postgres",
    [string]$Password = ""
)

Write-Host "`n🔌 Testing PostgreSQL Connection`n" -ForegroundColor Cyan

# Get password if not provided
if (-not $Password) {
    $securePassword = Read-Host "Enter PostgreSQL password for '$Username' user" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

$connectionString = "Host=$Host;Port=$Port;Database=$Database;Username=$Username;Password=$Password"

Write-Host "📊 Connection Details:" -ForegroundColor Cyan
Write-Host "  Host: $Host" -ForegroundColor Gray
Write-Host "  Port: $Port" -ForegroundColor Gray
Write-Host "  Database: $Database" -ForegroundColor Gray
Write-Host "  Username: $Username" -ForegroundColor Gray
Write-Host ""

# Try to load Npgsql
try {
    Write-Host "📦 Loading Npgsql library..." -ForegroundColor Yellow
    
    # Check multiple possible locations
    $npgsqlLocations = @(
        "D:\Program Files\PostgreSQL\17\Npgsql\Npgsql.dll",
        "$env:USERPROFILE\.nuget\packages\npgsql\10.0.1\lib\net8.0\Npgsql.dll",
        "D:\source\ecommerce-multiagent-project\backend\ECommerce.Data\bin\Release\net10.0\Npgsql.dll"
    )
    
    $npgsqlPath = $null
    foreach ($location in $npgsqlLocations) {
        if (Test-Path $location) {
            $npgsqlPath = $location
            break
        }
    }
    
    if ($npgsqlPath) {
        Write-Host "  Found: $npgsqlPath" -ForegroundColor Gray
        Add-Type -Path $npgsqlPath
    } else {
        Write-Host "⚠️  Npgsql.dll not found in standard locations" -ForegroundColor Yellow
        Write-Host "  Trying direct connection with psql..." -ForegroundColor Yellow
        
        # Try using psql command instead
        $psqlPath = "D:\Program Files\PostgreSQL\17\bin\psql.exe"
        if (Test-Path $psqlPath) {
            $env:PGPASSWORD = $Password
            $result = & $psqlPath -h $Host -p $Port -U $Username -d $Database -c "SELECT version();" 2>&1
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "`n✅ Connection successful using psql!" -ForegroundColor Green
                Write-Host "  PostgreSQL Version:" -ForegroundColor Green
                Write-Host "  $($result[2])" -ForegroundColor Gray
                
                # Get table count
                $tableCount = & $psqlPath -h $Host -p $Port -U $Username -d $Database -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';" 2>&1
                Write-Host "`n📊 Database Statistics:" -ForegroundColor Cyan
                Write-Host "  Tables: $($tableCount.Trim())" -ForegroundColor Gray
                
                $env:PGPASSWORD = $null
                exit 0
            } else {
                Write-Host "`n❌ Connection failed: $result" -ForegroundColor Red
                $env:PGPASSWORD = $null
                exit 1
            }
        } else {
            Write-Host "❌ psql.exe not found" -ForegroundColor Red
            exit 1
        }
    }
} catch {
    Write-Host "⚠️  Error loading Npgsql: $_" -ForegroundColor Yellow
    Write-Host "Continuing with alternative method..." -ForegroundColor Yellow
}

# Try .NET connection
Write-Host "`n🔌 Attempting connection..." -ForegroundColor Yellow

try {
    $conn = New-Object Npgsql.NpgsqlConnection($connectionString)
    $conn.Open()
    
    Write-Host "✅ Connection successful!`n" -ForegroundColor Green
    
    Write-Host "📊 Server Information:" -ForegroundColor Cyan
    Write-Host "  Database: $($conn.Database)" -ForegroundColor Green
    Write-Host "  Server Version: $($conn.ServerVersion)" -ForegroundColor Green
    Write-Host "  State: $($conn.State)" -ForegroundColor Green
    
    # Get table count
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public'"
    $tableCount = $cmd.ExecuteScalar()
    
    Write-Host "`n📋 Database Statistics:" -ForegroundColor Cyan
    Write-Host "  Tables: $tableCount" -ForegroundColor Gray
    
    # Get view count
    $cmd.CommandText = "SELECT COUNT(*) FROM information_schema.views WHERE table_schema = 'public'"
    $viewCount = $cmd.ExecuteScalar()
    Write-Host "  Views: $viewCount" -ForegroundColor Gray
    
    # Check if migrations have run
    $cmd.CommandText = "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'users')"
    $migrationsRun = $cmd.ExecuteScalar()
    
    if ($migrationsRun) {
        Write-Host "`n✅ Migrations have been applied" -ForegroundColor Green
        
        # Get record counts
        $cmd.CommandText = "SELECT COUNT(*) FROM users"
        $userCount = $cmd.ExecuteScalar()
        Write-Host "  Users: $userCount" -ForegroundColor Gray
        
        $cmd.CommandText = "SELECT COUNT(*) FROM products"
        $productCount = $cmd.ExecuteScalar()
        Write-Host "  Products: $productCount" -ForegroundColor Gray
        
        $cmd.CommandText = "SELECT COUNT(*) FROM categories"
        $categoryCount = $cmd.ExecuteScalar()
        Write-Host "  Categories: $categoryCount" -ForegroundColor Gray
    } else {
        Write-Host "`n⚠️  Migrations not yet applied" -ForegroundColor Yellow
        Write-Host "  Run: .\run-migrations.ps1" -ForegroundColor Yellow
    }
    
    $conn.Close()
    Write-Host "`n🎉 Test completed successfully!`n" -ForegroundColor Green
    
} catch {
    Write-Host "`n❌ Connection failed!" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host "`n💡 Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Check if PostgreSQL service is running:" -ForegroundColor Gray
    Write-Host "     Get-Service postgresql-x64-17" -ForegroundColor Gray
    Write-Host "  2. Verify database 'ecommerce' exists" -ForegroundColor Gray
    Write-Host "  3. Check username and password" -ForegroundColor Gray
    Write-Host "  4. Ensure port 5432 is not blocked`n" -ForegroundColor Gray
    exit 1
}
