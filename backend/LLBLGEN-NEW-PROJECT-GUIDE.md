# LLBLGen Pro Designer - Complete Setup Guide

## 📋 Step-by-Step Instructions for New Project Dialog

When you open LLBLGen Pro Designer and go to **File > New Project**, you'll see several fields. Here's exactly what to enter:

---

## 🔧 **New Project Dialog Fields:**

### **1. Name:**
```
ECommerceProject
```
*(This is your project name - can be anything descriptive)*

### **2. Creator:**
```
Your Name
```
*(Optional - just put your name or leave default)*

### **3. Location:**
```
D:\source\ecommerce-multiagent-project\backend\ECommerce.Data
```
*(This is where the .llblgenproj file will be saved)*

**Important:** Save it in the ECommerce.Data folder so generated code goes in the right place!

### **4. Target Framework:**
```
LLBLGen Pro Runtime Framework
```
*(Select this from the dropdown)*

### **5. Target Framework Version:**
```
5.12 or 5.13
```
*(Match your installed version)*

### **6. Template Group:**
```
Adapter
```
⚠️ **CRITICAL:** Must select **Adapter** (NOT Self-Servicing)!

### **7. Template Set:**
```
SD.LLBLGen.Pro.DBDrivers.PostgreSql
```
*(Select PostgreSQL from the dropdown)*

### **8. .NET Framework Version:**
```
.NET 8.0 (or .NET 10.0 if available)
```
*(Match your ECommerce.Data project target - currently net10.0)*

### **9. Initial Catalog to Add (Optional):**
```
Leave empty for now
```
*(We'll add the database connection manually in next steps)*

### **10. Root Namespace:**
```
ECommerce.Data
```
⚠️ **CRITICAL:** This MUST match your project namespace!

### **11. Entity Base Class Name:**
```
CommonEntityBase
```
*(Standard base class for all generated entities)*

---

## 📊 **Quick Reference - What to Fill:**

| Field | Value |
|-------|-------|
| Name | `ECommerceProject` |
| Creator | `Your Name` |
| Location | `D:\source\ecommerce-multiagent-project\backend\ECommerce.Data` |
| Target Framework | `LLBLGen Pro Runtime Framework` |
| Template Group | **`Adapter`** ⚠️ |
| Template Set | `SD.LLBLGen.Pro.DBDrivers.PostgreSql` |
| .NET Version | `.NET 8.0` or `.NET 10.0` |
| Root Namespace | `ECommerce.Data` |
| Entity Base Class | `CommonEntityBase` |

---

## ✅ **After Clicking "Create":**

The project will be created. Next, you'll need to:

### **Step 1: Add Database Connection**

1. In the **Catalog Explorer** (left panel), right-click on the project root
2. Select **"Add New Database Catalog"**
3. You'll see a connection dialog - fill it in:

```
Driver Type: PostgreSQL
Server: localhost
Port: 5432
Database: ecommerce
User ID: postgres
Password: [your postgres password]
```

4. Click **"Test Connection"** - should show success!
5. Click **"OK"**

---

### **Step 2: Retrieve Schema (Add Tables)**

1. In **Catalog Explorer**, you should see your database connection
2. Right-click on the database node → Select **"Retrieve Catalog Data"**
3. A dialog will appear showing all available tables
4. **SELECT ALL 16 tables:**
   - ✅ addresses
   - ✅ cart_items
   - ✅ carts
   - ✅ categories
   - ✅ coupon_usages
   - ✅ coupons
   - ✅ order_items
   - ✅ orders
   - ✅ payments
   - ✅ product_images
   - ✅ products
   - ✅ reviews
   - ✅ saved_payment_methods
   - ✅ users
   - ✅ wishlist_items
   - ✅ wishlists

5. Click **"OK"** - tables will be imported

---

### **Step 3: Configure Entity Settings**

1. Go to **Project → Project Settings** (or press F4)
2. In the **Properties** window, configure:

#### **Entity Model:**
- **Entity Name Pattern:** `{TableName}Entity`
  - Example: `users` → `UserEntity`
- **Entity Field Name Pattern:** `{FieldName}`
  - Example: `user_id` → `UserId` (auto PascalCase conversion)

#### **Naming Conventions:**
- **Pattern Matching:** Set to **"Database-first"**
- **Casing:** Set to **"Pascal Case"** for C# properties
- **Remove Underscores:** ✅ Check this box
  - `user_id` → `UserId`
  - `created_at` → `CreatedAt`
  - `product_images` → `ProductImages`

#### **Code Generation:**
- **Generate Type-Shortcut Properties:** ✅ Yes
- **Generate XML Doc Comments:** ✅ Yes (optional but recommended)
- **Target Language:** C#
- **Target .NET Version:** .NET 8.0 or .NET 10.0

#### **Adapter Specific:**
- **Generate Adapter Classes:** ✅ Yes
- **Generate TypedView Classes:** ❌ No (we don't need these)
- **Generate Stored Procedure Call Methods:** ❌ No (not using SPs yet)

---

### **Step 4: Configure Output Paths**

1. Still in **Project Settings**, go to **Output** tab:

#### **Entity Classes:**
```
Output folder: EntityClasses
```

#### **Database Specific:**
```
Output folder: DatabaseSpecific
```

#### **Helper Classes:**
```
Output folder: HelperClasses
```

These folders will be created automatically in:
```
D:\source\ecommerce-multiagent-project\backend\ECommerce.Data\
```

---

### **Step 5: Set Up Relations**

LLBLGen should auto-detect foreign key relationships. Verify:

1. Click on any entity (e.g., `Products`)
2. Look at **Navigator** tab (bottom)
3. You should see relations like:
   - `Products.category_id → Categories.category_id`
   - `Products.product_id ← ProductImages.product_id`
   - `Products.product_id ← Reviews.product_id`

If relations are missing:
1. Right-click in **Navigator** → **"Add Relation"**
2. Manually define FK relationships

---

### **Step 6: Generate Code!**

1. Press **F5** or go to **Project → Generate Source Code**
2. A progress window will appear
3. Generation should complete in 5-10 seconds
4. Check for any errors in the **Output** tab

**Expected Output:**
```
Generated files:
- EntityClasses/UserEntity.cs
- EntityClasses/ProductEntity.cs
- EntityClasses/CategoryEntity.cs
- ... (16 entity files total)
- DatabaseSpecific/DataAccessAdapter.cs
- DatabaseSpecific/EntityFields2.cs
- DatabaseSpecific/FieldInfoProvider.cs
- HelperClasses/EntityFactory.cs
- ... (various helper files)
```

---

## 🎯 **What You'll Get:**

After generation, your folder structure will look like:

```
ECommerce.Data/
├── ECommerceProject.llblgenproj  ← LLBLGen project file
├── EntityClasses/
│   ├── UserEntity.cs
│   ├── ProductEntity.cs
│   ├── CategoryEntity.cs
│   ├── OrderEntity.cs
│   ├── CartEntity.cs
│   ├── CartItemEntity.cs
│   ├── OrderItemEntity.cs
│   ├── ReviewEntity.cs
│   ├── AddressEntity.cs
│   ├── ProductImageEntity.cs
│   ├── CouponEntity.cs
│   ├── CouponUsageEntity.cs
│   ├── WishlistEntity.cs
│   ├── WishlistItemEntity.cs
│   ├── PaymentEntity.cs
│   └── SavedPaymentMethodEntity.cs
├── DatabaseSpecific/
│   ├── DataAccessAdapter.cs       ← Main DB access class
│   ├── EntityFields2.cs           ← Field definitions
│   ├── FieldInfoProvider.cs
│   └── RelationClasses.cs         ← Relationship info
├── HelperClasses/
│   ├── EntityFactory.cs
│   └── FactoryClasses.cs
├── Repositories/
│   ├── BaseRepository.cs          ← Already exists
│   └── IProductRepository.cs      ← Already exists
└── Migrations/
    └── ... (existing migrations)
```

---

## ✅ **Verification Steps:**

After generation:

1. **Check File Count:**
   ```powershell
   cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data
   Get-ChildItem -Recurse -Filter "*Entity.cs" | Measure-Object
   ```
   Should show **16 entity files**

2. **Build Project:**
   ```powershell
   dotnet build
   ```
   Should succeed with **0 errors** (previously had 16 errors)

3. **Check Entity Example:**
   Open `EntityClasses/UserEntity.cs` - should look like:
   ```csharp
   public partial class UserEntity : CommonEntityBase
   {
       public Guid UserId { get; set; }
       public string Email { get; set; }
       public string PasswordHash { get; set; }
       public string FirstName { get; set; }
       // ... more properties
   }
   ```

---

## 🚨 **Common Issues & Solutions:**

### **Issue 1: "Connection failed"**
**Solution:** Verify PostgreSQL is running:
```powershell
Get-Service postgresql-x64-17
```

### **Issue 2: "No tables found"**
**Solution:** Check database has tables:
```powershell
.\test-db-connection.ps1
```

### **Issue 3: "Wrong namespace generated"**
**Solution:** Check **Root Namespace** setting = `ECommerce.Data`

### **Issue 4: "Relations not detected"**
**Solution:** Check foreign keys exist in database:
```sql
SELECT * FROM information_schema.table_constraints 
WHERE constraint_type = 'FOREIGN KEY';
```

### **Issue 5: "Build still fails after generation"**
**Solution:** Clean and rebuild:
```powershell
dotnet clean
dotnet build
```

---

## 🎓 **Important LLBLGen Concepts:**

1. **Adapter Pattern:**
   - Entities are POCOs (Plain Old CLR Objects)
   - `DataAccessAdapter` handles all DB operations
   - Clean separation of concerns

2. **PrefetchPath:**
   - Avoids N+1 query problems
   - Eagerly loads related entities
   - Example: Load product with images in one query

3. **RelationPredicateBucket:**
   - Type-safe query filtering
   - Replaces raw SQL WHERE clauses
   - Example: `bucket.Add(ProductFields.CategoryId == categoryId)`

4. **Entity Collections:**
   - `EntityCollection<ProductEntity>` for multiple results
   - Supports LINQ queries
   - Example: `products.Where(p => p.IsActive).ToList()`

---

## 📞 **Need Help?**

If you get stuck:
1. Check LLBLGen logs in **Output** tab (bottom panel)
2. Verify database connection with `.\test-db-connection.ps1`
3. Review generated code in `EntityClasses/` folder
4. Check that all 16 tables were imported

---

## 🎉 **Success Criteria:**

You'll know it worked when:
- ✅ 16 `*Entity.cs` files in `EntityClasses/` folder
- ✅ `DataAccessAdapter.cs` in `DatabaseSpecific/` folder
- ✅ `dotnet build` completes with **0 errors**
- ✅ Repository files (`BaseRepository.cs`) no longer have red squiggles

---

**Once generation is complete, let me know and I'll continue with:**
- Implementing remaining repositories
- Updating service layer
- Configuring dependency injection
- Testing the complete backend

Good luck! 🚀
