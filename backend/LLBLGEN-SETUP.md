# LLBLGen Pro Setup Guide

## Prerequisites
- LLBLGen Pro Designer 5.11+ installed
- PostgreSQL database with migrations applied

## Setup Steps

### 1. Run Migrations First
```bash
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data
dotnet tool install -g FluentMigrator.DotNet.Cli
dotnet build

# Run migrations (update connection string)
dotnet fm migrate -p postgres -c "Host=localhost;Database=ecommerce;Username=postgres;Password=yourpassword" -a bin/Debug/net10.0/ECommerce.Data.dll
```

### 2. Open LLBLGen Pro Designer
1. Launch **LLBLGen Pro Designer**
2. Click **File > New Project**
3. Select template: **Adapter** (not Self-Servicing)
4. Target framework: **.NET 8.0**
5. Save as: `D:\source\ecommerce-multiagent-project\backend\ECommerce.Data\LLBLGen\ECommerce.llblgenproj`

### 3. Configure Database Connection
1. In Designer, go to **Project > Catalog Explorer**
2. Click **Add new catalog**
3. Select **PostgreSQL**
4. Enter connection details:
   - Host: `localhost`
   - Database: `ecommerce`
   - Username: `postgres`
   - Password: `yourpassword`
5. Click **Refresh Catalog**

### 4. Retrieve Schema
1. In **Catalog Explorer**, expand your database
2. Right-click **Tables** > **Add tables to project**
3. Select all tables:
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
4. Click **OK**

### 5. Configure Entity Settings
1. Go to **Project > Project Settings**
2. Set **Root Namespace**: `ECommerce.Data`
3. Set **Entity Base Class Name**: `CommonEntityBase`
4. Set **Output Folder**: `DatabaseSpecific`
5. Set **Entity Output Folder**: `EntityClasses`
6. Enable **Generate Type Converters**: Yes
7. Enable **Generate Field Properties**: Yes

### 6. Set Naming Conventions
1. Go to **Tools > Preferences > Project**
2. **Entity Naming**:
   - Remove plural: Yes
   - Pascal case: Yes
   - Example: `users` table → `UserEntity` class
3. **Field Naming**:
   - Pascal case: Yes
   - Example: `first_name` column → `FirstName` property

### 7. Generate Code
1. Click **Project > Generate Source Code** (or F5)
2. Wait for generation to complete
3. Files will be created in:
   - `DatabaseSpecific/` - Data Access Adapter, Field definitions
   - `EntityClasses/` - Entity classes (UserEntity, ProductEntity, etc.)
   - `TypedListClasses/` - Typed lists
   - `HelperClasses/` - Helper code

### 8. Verify Generated Files
Check that these folders now exist:
```
ECommerce.Data/
├── LLBLGen/
│   └── ECommerce.llblgenproj
├── DatabaseSpecific/
│   ├── DataAccessAdapter.cs
│   ├── EntityFields2.cs
│   ├── FieldInfoProvider.cs
│   └── ...
├── EntityClasses/
│   ├── UserEntity.cs
│   ├── ProductEntity.cs
│   ├── OrderEntity.cs
│   └── ...
└── Migrations/
    └── V1_InitialSchema.cs
```

### 9. Update Repository Pattern
Repositories should now use LLBLGen entities:

```csharp
using SD.LLBLGen.Pro.ORMSupportClasses;
using ECommerce.Data.EntityClasses;
using ECommerce.Data.DatabaseSpecific;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<UserEntity> GetByIdAsync(Guid id)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var user = new UserEntity(id);
        await adapter.FetchEntityAsync(user);
        return user.IsNew ? null : user;
    }

    public async Task<List<UserEntity>> GetAllAsync()
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var users = new EntityCollection<UserEntity>();
        await adapter.FetchEntityCollectionAsync(users, null);
        return users;
    }

    public async Task<bool> SaveAsync(UserEntity user)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        return await adapter.SaveEntityAsync(user);
    }
}
```

## Configuration in Program.cs

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "ECommerceDb": "Host=localhost;Database=ecommerce;Username=postgres;Password=yourpassword"
  }
}

// Program.cs
builder.Services.AddScoped<IUserRepository>(sp => 
    new UserRepository(builder.Configuration.GetConnectionString("ECommerceDb")));
```

## Benefits of LLBLGen Pro
✅ **Type-safe queries** - No string-based SQL
✅ **Adapter pattern** - Entities decoupled from persistence
✅ **Change tracking** - Automatic dirty field detection  
✅ **Lazy loading** - Efficient navigation properties
✅ **Prefetch paths** - Prevent N+1 queries
✅ **Full LINQ support** - Complex queries with IntelliSense

## Next Steps
1. Install LLBLGen Pro Designer if not already installed
2. Follow steps above to generate entities
3. Update repositories to use generated `EntityClasses`
4. Test with DataAccessAdapter
5. Build and verify solution compiles

## Support
- LLBLGen Pro Docs: https://www.llblgen.com/Documentation/5.11/
- Forums: https://www.llblgen.com/tinyforum/
