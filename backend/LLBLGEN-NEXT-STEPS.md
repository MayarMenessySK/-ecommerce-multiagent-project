# 🎯 LLBLGen Pro - Complete Setup Guide

## Current Status: ✅ Project Created, ⏸️ Tables Not Added Yet

I can see you've created `ecommerce.llblgenproj` and configured the database connection!

**What's done:**
- ✅ LLBLGen project file exists
- ✅ Database connection configured
- ⏸️ Need to add tables
- ⏸️ Need to generate code

---

## 📋 Step-by-Step Instructions

### **Step 1: Open Your Project in LLBLGen Designer**

```
1. Open: C:\Program Files (x86)\Solutions Design\LLBLGen Pro v5.12\LLBLGen Pro.exe
2. File → Open Project
3. Navigate to: D:\source\ecommerce-multiagent-project\backend\ECommerce.Data\ecommerce.llblgenproj
4. Click Open
```

---

### **Step 2: Retrieve Schema from Database**

**In the LLBLGen Designer window:**

1. Look for **"Catalog Explorer"** panel (usually on the right side)
   
2. You should see your PostgreSQL connection listed
   
3. **IMPORTANT:** Right-click on the database name → **"Set Schemas to Fetch"**
   - ✅ **CHECK the "public" checkbox**
   - ❌ Uncheck all others
   - Click OK

4. Right-click again → **"Refresh Catalogs"**
   
5. Expand the database → Expand "public" schema
   
6. You should now see **16 tables**:
   ```
   ├── addresses
   ├── carts
   ├── cart_items
   ├── categories
   ├── coupons
   ├── coupon_usage
   ├── orders
   ├── order_items
   ├── payments
   ├── products
   ├── product_images
   ├── reviews
   ├── users
   ├── wishlists
   ├── wishlist_items
   └── (+ 7 views - optional)
   ```

**If tables don't show up:** See troubleshooting at the end!

---

### **Step 3: Add Tables to Project**

1. In **Catalog Explorer**, select all 16 tables:
   - Click the first table (`addresses`)
   - Hold **SHIFT** and click the last table (`wishlist_items`)
   - All 16 tables should be highlighted

2. **Drag** them to the **Project Explorer** panel (usually on the left)
   - Or right-click → "Add to Project"

3. You'll see a dialog asking about table naming:
   - **Entity Naming:** PascalCase (recommended)
   - **Remove underscores:** Yes
   - Click OK

4. **Project Explorer** should now show all entities:
   ```
   Project
   ├── Address
   ├── Cart
   ├── CartItem
   ├── Category
   ├── Coupon
   ├── CouponUsage
   ├── Order
   ├── OrderItem
   ├── Payment
   ├── Product
   ├── ProductImage
   ├── Review
   ├── User
   ├── Wishlist
   └── WishlistItem
   ```

---

### **Step 4: Configure Output Settings (Optional but Recommended)**

1. Go to **Project → Settings** (or press F7)

2. Check these settings:
   ```
   General Settings:
   ✅ Target Framework: .NET 10.0
   ✅ Root Namespace: ECommerce.Data
   
   Output Settings:
   ✅ Output Path: [Should point to your ECommerce.Data folder]
   ✅ Template Group: Adapter
   
   Entity Settings:
   ✅ Generate Entity Interfaces: Yes (optional but recommended)
   ✅ Generate Partial Classes: Yes
   ```

3. Click OK

---

### **Step 5: Generate Code** 🚀

1. Click the **"Generate Code"** button in the toolbar
   - It looks like a ⚡ lightning bolt
   - Or: **Project → Generate Source Code** (F5)

2. Wait for generation to complete (5-10 seconds)

3. You should see output like:
   ```
   Generating entity: Address
   Generating entity: Cart
   Generating entity: Category
   ...
   Generation complete! 
   Generated: 150+ files
   ```

---

### **Step 6: Verify Generated Files**

Close LLBLGen Designer and check these folders in your file explorer:

```powershell
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data

# Should have MANY files now
Get-ChildItem EntityClasses -File *.cs | Measure-Object | Select-Object Count
# Expected: 15+ files

Get-ChildItem HelperClasses -File *.cs | Measure-Object | Select-Object Count
# Expected: 50+ files

Get-ChildItem FactoryClasses -File *.cs | Measure-Object | Select-Object Count
# Expected: 15+ files

Get-ChildItem Linq -File *.cs | Measure-Object | Select-Object Count
# Expected: 1 file (LinqMetaData.cs) - should be MUCH bigger now
```

---

### **Step 7: Remove Placeholder Files**

Now that LLBLGen has generated real files, remove my placeholders:

```powershell
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data

# Check if LinqMetaData was regenerated properly
$linq = Get-Content "Linq\LinqMetaData.cs" -Raw
if ($linq -match "NotImplementedException") {
    Write-Host "⚠️ Still using placeholder - generation may have failed" -ForegroundColor Yellow
} else {
    Write-Host "✅ Real LLBLGen code generated!" -ForegroundColor Green
}
```

**If it says "Real LLBLGen code generated", you're done with this step!**

---

### **Step 8: Build the Project**

```powershell
cd D:\source\ecommerce-multiagent-project\backend

# Build Data project
dotnet build ECommerce.Data/ECommerce.Data.csproj

# Should succeed with 0 errors!
```

**Expected output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

### **Step 9: Build Everything**

```powershell
# Build DatabaseSpecific
dotnet build ECommerce.Data.DatabaseSpecific/ECommerce.Data.DatabaseSpecific.csproj

# Build Core (repositories)
dotnet build ECommerce.Core/ECommerce.Core.csproj

# Build entire solution
dotnet build
```

**All should succeed!** 🎉

---

## 🔥 What To Do After Generation

Once all projects build successfully, **we move to Phase 5**:

### **Phase 5: Update ECommerce.API**

I'll help you:
1. Configure dependency injection in `Program.cs`
2. Register `DataAccessAdapter` with connection string
3. Register all repositories
4. Update controllers to use new repositories
5. Test API endpoints

---

## ⚠️ Troubleshooting

### **Problem: Tables Don't Show in Catalog Explorer**

**Solution:**
1. Right-click database → "Set Schemas to Fetch"
2. ✅ CHECK "public"
3. Right-click → "Refresh Catalogs"

See: `LLBLGEN-TABLES-NOT-SHOWING.md` for detailed fix

---

### **Problem: Generation Failed**

**Check:**
1. PostgreSQL service is running
2. Database connection works (test it in LLBLGen)
3. All 16 tables are added to project
4. Output path is correct

---

### **Problem: Build Errors After Generation**

```powershell
# Clean and rebuild
cd D:\source\ecommerce-multiagent-project\backend
dotnet clean
dotnet build
```

If still failing, show me the error messages.

---

## 📊 What You Should See After Generation

### **EntityClasses Folder:**
```
AddressEntity.cs           ← Full entity with all properties
CartEntity.cs
CartItemEntity.cs
CategoryEntity.cs
CouponEntity.cs
CouponUsageEntity.cs
OrderEntity.cs
OrderItemEntity.cs
PaymentEntity.cs
ProductEntity.cs           ← Example: Should have 25+ properties
ProductImageEntity.cs
ReviewEntity.cs
UserEntity.cs
WishlistEntity.cs
WishlistItemEntity.cs
```

### **HelperClasses Folder:**
```
EntityFields2.cs
EntityFieldsCore.cs
EntityFieldsFactory.cs
PredicateExpression.cs
... 50+ files
```

### **Each Entity Should Look Like:**

```csharp
// OLD (my placeholder):
public partial class ProductEntity : EntityBase2
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// NEW (LLBLGen generated):
public partial class ProductEntity : CommonEntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Sku { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string Currency { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public Guid CategoryId { get; set; }
    public string Brand { get; set; }
    // ... 10+ more properties
    
    // Navigation properties
    public virtual CategoryEntity Category { get; set; }
    public virtual ICollection<ProductImageEntity> ProductImages { get; set; }
    public virtual ICollection<ReviewEntity> Reviews { get; set; }
    // ... etc
}
```

---

## 🎯 Next Steps After You Generate

**Tell me when generation is complete!**

I'll then help you:
1. ✅ Verify generated code
2. ✅ Configure API dependency injection
3. ✅ Test repositories
4. ✅ Complete Phase 5 & 6

---

## 📞 Need Help?

**If you get stuck at any step, show me:**
1. Screenshot of LLBLGen Designer
2. Error messages (if any)
3. Output of this command:
```powershell
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data
Get-ChildItem -Recurse -File *.cs | Group-Object Directory | Select-Object Name, Count
```

---

**You're almost there! Just need to add tables and click Generate!** 🚀
