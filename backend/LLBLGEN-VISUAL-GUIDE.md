# LLBLGen Pro - Step-by-Step Visual Guide

## 🎯 **The Complete Process in 6 Steps**

---

## **Step 1: Create New Project** ✅

**File → New Project**

```
┌─────────────────────────────────────────────┐
│ New Project                                 │
├─────────────────────────────────────────────┤
│ Name: ECommerceProject                      │
│ Location: D:\source\ecommerce-...\backend\  │
│                      ECommerce.Data         │
│                                             │
│ Template Group: ⚠️ Adapter (NOT Self-Serv.)│
│ Template Set: PostgreSQL                    │
│ .NET Version: 8.0 or 10.0                   │
│ Root Namespace: ECommerce.Data              │
│                                             │
│           [Cancel]  [Create]                │
└─────────────────────────────────────────────┘
```

**Result:** `.llblgenproj` file created

---

## **Step 2: Add Database Connection** 🔌

**Catalog Explorer → Right-click → Add New Catalog**

```
┌─────────────────────────────────────────────┐
│ Database Connection                         │
├─────────────────────────────────────────────┤
│ Driver: PostgreSQL                          │
│ Server: localhost                           │
│ Port: 5432                                  │
│ Database: ecommerce                         │
│ User: postgres                              │
│ Password: ********                          │
│                                             │
│ Schema: public ✅                           │
│                                             │
│        [Test Connection]  [OK]              │
└─────────────────────────────────────────────┘
```

**Click "Test Connection"** → Should say "Success!"

---

## **Step 3: Retrieve Tables** 📋

**Catalog Explorer → Right-click database → Refresh Catalogs**

You should see:

```
📁 Catalog Explorer
  └─ 🔌 localhost:5432/ecommerce
      └─ 📁 public (schema)
          ├─ 📊 addresses
          ├─ 📊 cart_items
          ├─ 📊 carts
          ├─ 📊 categories
          ├─ 📊 coupon_usages
          ├─ 📊 coupons
          ├─ 📊 order_items
          ├─ 📊 orders
          ├─ 📊 payments
          ├─ 📊 product_images
          ├─ 📊 products
          ├─ 📊 reviews
          ├─ 📊 saved_payment_methods
          ├─ 📊 users
          ├─ 📊 wishlist_items
          └─ 📊 wishlists
```

**Total: 16 tables** ✅

---

## **Step 4: Add Tables to Project** ⚠️ **CRITICAL**

**Select all 16 tables → Right-click → "Add to project"**

Before (only in Catalog):
```
📁 Catalog Explorer          📁 Project Explorer
  └─ 📊 users                   └─ (empty)
  └─ 📊 products
  └─ ...
```

After (in both):
```
📁 Catalog Explorer          📁 Project Explorer
  └─ 📊 users                   └─ 📦 UserEntity
  └─ 📊 products                └─ 📦 ProductEntity
  └─ ...                        └─ 📦 CategoryEntity
                                └─ ... (16 entities)
```

**If Project Explorer is empty, you skipped this step!**

---

## **Step 5: Configure Naming** 📝

**Project → Project Settings (F4) → Conventions**

```
┌─────────────────────────────────────────────┐
│ Naming Conventions                          │
├─────────────────────────────────────────────┤
│ Entity Name Pattern:                        │
│   {$ElementName}Entity                      │
│                                             │
│ Field Name Pattern:                         │
│   {$FieldName}                              │
│                                             │
│ Element Name Casing: PascalCase             │
│ Field Name Casing: PascalCase               │
│                                             │
│ ✅ Remove underscores from names            │
│ ✅ Make element names singular              │
│                                             │
│              [Apply]  [OK]                  │
└─────────────────────────────────────────────┘
```

**Result:**
- `users` → `UserEntity`
- `user_id` → `UserId`
- `created_at` → `CreatedAt`

---

## **Step 6: Set Output Folders** 📂

**Project → Project Settings → Output**

```
┌─────────────────────────────────────────────┐
│ Output Settings                             │
├─────────────────────────────────────────────┤
│ Entity Classes:                             │
│   Output folder: EntityClasses              │
│                                             │
│ Database Specific:                          │
│   Output folder: DatabaseSpecific           │
│                                             │
│ Helper Classes:                             │
│   Output folder: HelperClasses              │
│                                             │
│ Root namespace: ECommerce.Data              │
│ Target .NET: 8.0 or 10.0                    │
│                                             │
│              [Apply]  [OK]                  │
└─────────────────────────────────────────────┘
```

---

## **Step 7: Generate Code!** 🚀

**Press F5 or Project → Generate Source Code**

```
┌─────────────────────────────────────────────┐
│ Code Generation Progress                    │
├─────────────────────────────────────────────┤
│ ⏳ Analyzing entities...                    │
│ ⏳ Generating UserEntity.cs...              │
│ ⏳ Generating ProductEntity.cs...           │
│ ⏳ Generating CategoryEntity.cs...          │
│ ...                                         │
│ ⏳ Generating DataAccessAdapter.cs...       │
│                                             │
│ ✅ Generation completed successfully!       │
│    16 entities generated                    │
│    Generated 45 files                       │
│                                             │
│                    [Close]                  │
└─────────────────────────────────────────────┘
```

---

## **Step 8: Verify Files** ✅

```
ECommerce.Data/
├─ ECommerceProject.llblgenproj ✅
│
├─ EntityClasses/ ✅
│  ├─ UserEntity.cs
│  ├─ ProductEntity.cs
│  ├─ CategoryEntity.cs
│  ├─ OrderEntity.cs
│  ├─ CartEntity.cs
│  ├─ CartItemEntity.cs
│  ├─ OrderItemEntity.cs
│  ├─ ReviewEntity.cs
│  ├─ AddressEntity.cs
│  ├─ ProductImageEntity.cs
│  ├─ CouponEntity.cs
│  ├─ CouponUsageEntity.cs
│  ├─ WishlistEntity.cs
│  ├─ WishlistItemEntity.cs
│  ├─ PaymentEntity.cs
│  └─ SavedPaymentMethodEntity.cs
│
├─ DatabaseSpecific/ ✅
│  ├─ DataAccessAdapter.cs
│  ├─ EntityFields2.cs
│  ├─ FieldInfoProvider.cs
│  └─ RelationClasses.cs
│
└─ HelperClasses/ ✅
   ├─ EntityFactory.cs
   └─ FactoryClasses.cs
```

**Check file count:**
```powershell
Get-ChildItem EntityClasses\*Entity.cs | Measure-Object
# Should show: Count : 16
```

---

## **Step 9: Build Project** 🔨

```powershell
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data
dotnet build
```

**Expected:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**Before LLBLGen:** 16 errors (missing entities)
**After LLBLGen:** 0 errors ✅

---

## 🎯 **Success Checklist**

- ✅ Project created with Adapter template
- ✅ Database connection successful
- ✅ 16 tables visible in Catalog Explorer
- ✅ 16 entities visible in Project Explorer (added to project!)
- ✅ Naming: snake_case → PascalCase
- ✅ F5 generation completed
- ✅ 16 *Entity.cs files in EntityClasses/
- ✅ DataAccessAdapter.cs in DatabaseSpecific/
- ✅ `dotnet build` succeeds with 0 errors

---

## 🚨 **Common Mistakes**

❌ **Forgot to add tables to project**
   → Tables in Catalog but not Project Explorer
   
❌ **Selected Self-Servicing instead of Adapter**
   → Wrong pattern, need to start over

❌ **Wrong namespace**
   → Check Project Settings → Root Namespace

❌ **Output folders not set**
   → Files generated to wrong location

❌ **Didn't press F5**
   → No files generated even though setup is correct

---

## 📞 **Having Issues?**

Check: `LLBLGEN-TROUBLESHOOTING.md` for detailed solutions!

**Tell me which step you're stuck on and I'll help!** 🚀
