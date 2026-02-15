# PostgreSQL Database Setup for E-Commerce Project

## 🎯 Current Status
- ✅ PostgreSQL 17 is installed and running
- ✅ Location: `D:\Program Files\PostgreSQL\17`
- ✅ FluentMigrator CLI tool installed

## 📝 Step 1: Create Database

### Option A: Using Command Line (psql)

Open a new PowerShell window and run:

```powershell
# Set PostgreSQL path
$env:Path += ";D:\Program Files\PostgreSQL\17\bin"

# Connect to PostgreSQL (will prompt for password)
psql -U postgres

# Once connected, create the database:
CREATE DATABASE ecommerce;

# Grant permissions (if needed)
GRANT ALL PRIVILEGES ON DATABASE ecommerce TO postgres;

# List databases to verify
\l

# Exit psql
\q
```

### Option B: Using pgAdmin 4

1. Launch pgAdmin 4 (should be in Start Menu)
2. Connect to PostgreSQL 17 server
3. Right-click on "Databases" > "Create" > "Database..."
4. Database name: `ecommerce`
5. Owner: `postgres`
6. Click "Save"

## 📝 Step 2: Configure Connection String

### Update appsettings.json

Edit: `D:\source\ecommerce-multiagent-project\backend\ECommerce.API\appsettings.json`

Add this configuration (update password):

```json
{
  "ConnectionStrings": {
    "ECommerceDb": "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=YOUR_PASSWORD_HERE"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Create appsettings.Development.json (for local development)

Create: `D:\source\ecommerce-multiagent-project\backend\ECommerce.API\appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "ECommerceDb": "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=YOUR_PASSWORD_HERE"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

**⚠️ IMPORTANT:** 
- Never commit `appsettings.Development.json` with real passwords
- Add to `.gitignore`: `appsettings.Development.json`

## 📝 Step 3: Run Migrations

### Method 1: Using FluentMigrator CLI (Recommended)

```powershell
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data

# Build the project first
dotnet build --configuration Release

# Run migrations (update connection string)
dotnet fm migrate -p postgres `
  -c "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=YOUR_PASSWORD" `
  -a "bin\Release\net10.0\ECommerce.Data.dll"
```

### Method 2: Create Migration Runner in Program.cs

Add to `ECommerce.API/Program.cs`:

```csharp
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);

// Add FluentMigrator services
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("ECommerceDb"))
        .ScanIn(typeof(ECommerce.Data.Migrations.V1_InitialSchema).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

var app = builder.Build();

// Run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.Run();
```

### Method 3: Manual Migration Script

Create: `D:\source\ecommerce-multiagent-project\backend\run-migrations.ps1`

```powershell
# PostgreSQL Migration Runner
param(
    [string]$ConnectionString = "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=postgres",
    [string]$Version = ""
)

Write-Host "🔄 Running database migrations..." -ForegroundColor Cyan

# Build the project
Write-Host "📦 Building ECommerce.Data project..." -ForegroundColor Yellow
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful" -ForegroundColor Green

# Run migrations
Write-Host "🚀 Applying migrations..." -ForegroundColor Yellow

if ($Version) {
    dotnet fm migrate -p postgres -c $ConnectionString -a "bin\Release\net10.0\ECommerce.Data.dll" --version $Version
} else {
    dotnet fm migrate -p postgres -c $ConnectionString -a "bin\Release\net10.0\ECommerce.Data.dll"
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Migrations applied successfully!" -ForegroundColor Green
} else {
    Write-Host "❌ Migration failed!" -ForegroundColor Red
    exit 1
}
```

Usage:
```powershell
# Run all migrations
.\run-migrations.ps1 -ConnectionString "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=yourpass"

# Run specific version
.\run-migrations.ps1 -ConnectionString "..." -Version 1
```

## 📝 Step 4: Verify Migrations

### Check Tables Created

```powershell
# Add PostgreSQL to path
$env:Path += ";D:\Program Files\PostgreSQL\17\bin"

# Connect to database
psql -U postgres -d ecommerce

# List all tables
\dt

# Check specific table
\d users

# Count records in a table
SELECT COUNT(*) FROM users;

# Exit
\q
```

### Expected Tables After All Migrations:

**V1_InitialSchema (10 tables):**
- users
- addresses
- categories
- products
- product_images
- carts
- cart_items
- orders
- order_items
- reviews

**V2_AddCoupons (2 tables):**
- coupons
- coupon_usages

**V3_AddWishlists (2 tables):**
- wishlists
- wishlist_items

**V4_AddPayments (2 tables):**
- payments
- saved_payment_methods

**V5_CreateViews (7 views):**
- product_inventory_view
- product_rating_view
- order_summary_view
- customer_orders_view
- cart_summary_view
- sales_performance_view
- category_performance_view

**V100_SeedInitialData:**
- Inserts sample data into above tables

**Total: 16 tables + 7 views**

## 📝 Step 5: Test Database Connection

Create a simple test file: `test-db-connection.ps1`

```powershell
$connectionString = "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=YOUR_PASSWORD"

Write-Host "Testing database connection..." -ForegroundColor Cyan

try {
    Add-Type -Path "D:\Program Files\PostgreSQL\17\Npgsql\Npgsql.dll"
    
    $conn = New-Object Npgsql.NpgsqlConnection($connectionString)
    $conn.Open()
    
    Write-Host "✅ Connection successful!" -ForegroundColor Green
    Write-Host "Database: $($conn.Database)" -ForegroundColor Green
    Write-Host "Server Version: $($conn.ServerVersion)" -ForegroundColor Green
    
    $conn.Close()
} catch {
    Write-Host "❌ Connection failed: $_" -ForegroundColor Red
}
```

## 🔒 Security Best Practices

### 1. Environment Variables

Create a `.env` file (add to .gitignore):

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=ecommerce
DB_USER=postgres
DB_PASSWORD=your_secure_password
```

### 2. User Secrets (for development)

```powershell
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.API

# Initialize user secrets
dotnet user-secrets init

# Set connection string
dotnet user-secrets set "ConnectionStrings:ECommerceDb" "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=yourpass"

# List secrets
dotnet user-secrets list
```

### 3. Azure Key Vault (for production)

```csharp
// In Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri("https://your-keyvault.vault.azure.net/"),
    new DefaultAzureCredential());
```

## 🐛 Troubleshooting

### Issue: "password authentication failed"

**Solution:** 
1. Check PostgreSQL password for `postgres` user
2. Reset if needed:
   ```powershell
   psql -U postgres
   ALTER USER postgres PASSWORD 'newpassword';
   ```

### Issue: "database does not exist"

**Solution:** Create the database first (see Step 1)

### Issue: "could not connect to server"

**Solution:** 
1. Check PostgreSQL service is running: `Get-Service postgresql-x64-17`
2. Start if needed: `Start-Service postgresql-x64-17`
3. Check if port 5432 is open: `netstat -an | findstr 5432`

### Issue: "permission denied"

**Solution:** Grant permissions:
```sql
GRANT ALL PRIVILEGES ON DATABASE ecommerce TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
```

### Issue: FluentMigrator fails with "assembly not found"

**Solution:**
1. Ensure project is built: `dotnet build --configuration Release`
2. Check assembly path is correct
3. Verify all NuGet packages are restored

## 📋 Quick Start Checklist

- [ ] PostgreSQL 17 is running
- [ ] Created `ecommerce` database
- [ ] Updated connection string in appsettings.json (or user-secrets)
- [ ] Built ECommerce.Data project
- [ ] Ran FluentMigrator CLI to apply migrations
- [ ] Verified tables exist in database
- [ ] Tested with seed data (V100)
- [ ] Ready for LLBLGen Pro entity generation

## 🎯 Next Steps

After database setup is complete:

1. ✅ Database is ready with all tables
2. ⏳ Generate LLBLGen Pro entities (manual Designer step)
3. ⏳ Complete repository implementations
4. ⏳ Update service layer
5. ⏳ Test backend API endpoints

---

**Need help with any of these steps? Let me know!** 🚀
