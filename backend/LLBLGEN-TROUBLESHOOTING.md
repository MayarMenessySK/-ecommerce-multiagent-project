# LLBLGen Pro - Common Issues & Solutions

## 🔧 **Issue 1: Tables Show Up But No Entities Generated**

**Problem:** You can see tables in the catalog but no entities are created.

**Solution:**
1. After retrieving tables, you must **add them to the project**
2. In Catalog Explorer → Right-click each table → **"Add to project"**
3. Or select all tables → Right-click → **"Add selected entities to project"**
4. Tables should now appear in the **Project Explorer** (not just Catalog)

---

## 🔧 **Issue 2: Wrong Naming Convention (snake_case not converting)**

**Problem:** Entities are named `users` instead of `User`, or `user_id` instead of `UserId`

**Solution - Set Naming Patterns:**

1. Go to **Project → Project Settings** (F4)
2. Find **"Conventions"** section
3. Set these patterns:

```
Entity Name Pattern: {$ElementName}Entity
Field Name Pattern: {$FieldName}

Pattern Settings:
- Element Name Casing: PascalCase
- Field Name Casing: PascalCase
- Remove Underscores: ✅ YES
```

**Before:**
- `users` → `users` ❌
- `user_id` → `user_id` ❌

**After:**
- `users` → `UserEntity` ✅
- `user_id` → `UserId` ✅

---

## 🔧 **Issue 3: Relations Not Detected**

**Problem:** Foreign keys exist but relations aren't showing in Navigator

**Solution:**

### **Option A: Auto-detect (Recommended)**
1. Select all tables in Project Explorer
2. Right-click → **"Set relationships"** → **"Detect from database"**
3. LLBLGen will scan foreign keys and create relations

### **Option B: Manual (If auto-detect fails)**
1. Click on entity (e.g., `Products`)
2. In Navigator panel → Right-click → **"Add Relation"**
3. Set:
   - **From:** ProductEntity.CategoryId
   - **To:** CategoryEntity.CategoryId
   - **Type:** Many-to-One
   - **Name:** Category

---

## 🔧 **Issue 4: Wrong Root Namespace**

**Problem:** Generated entities have wrong namespace like `SD.LLBLGen.Pro.Examples`

**Solution:**
1. Go to **Project → Project Settings**
2. Under **"Code Generation"** → **"General"**
3. Set **Root Namespace:** `ECommerce.Data`
4. **Delete old generated files** before regenerating

---

## 🔧 **Issue 5: Output Folders Not Created**

**Problem:** F5 generates but files go to wrong location

**Solution - Configure Output Settings:**

1. Go to **Project → Project Settings** → **"General"** tab
2. Set these paths:

```
Output Settings:
  Entity Classes → Output folder: EntityClasses
  Database Specific → Output folder: DatabaseSpecific  
  Helper Classes → Output folder: HelperClasses
  
  Root Output folder: (leave blank or set to ECommerce.Data folder)
```

---

## 🔧 **Issue 6: Generation Fails with Errors**

**Common Error Messages:**

### **"Template not found"**
**Cause:** Wrong template group selected
**Fix:** Use **Adapter** template, not Self-Servicing

### **"Database connection failed during generation"**
**Cause:** Connection string expired or DB unavailable
**Fix:** 
1. Test connection in Catalog Explorer
2. Right-click database → **"Refresh"**

### **"Field type not mapped"**
**Cause:** PostgreSQL type not recognized
**Fix:** 
1. Go to **Project → Type System**
2. Add custom type mappings if needed

---

## 🔧 **Issue 7: Guid vs UUID Mapping**

**Problem:** PostgreSQL `uuid` not mapping to C# `Guid`

**Solution:**
1. This should work automatically
2. If not, check **Project → Type System** → **PostgreSQL**
3. Ensure: `uuid` → `System.Guid`

---

## 🔧 **Issue 8: Tables Not Showing in Catalog**

**Problem:** Database connected but no tables visible

**Solution:**

### **Check Schema:**
1. Right-click database in Catalog Explorer
2. Select **"Set schemas to fetch"**
3. Ensure **"public"** is checked ✅
4. Click OK and refresh

### **Check Permissions:**
```sql
-- Run in psql or pgAdmin
GRANT SELECT ON ALL TABLES IN SCHEMA public TO postgres;
GRANT USAGE ON SCHEMA public TO postgres;
```

---

## 🔧 **Issue 9: Primary Keys Not Detected**

**Problem:** Entity generated but no primary key field

**Solution:**
1. Right-click entity in Project Explorer
2. Select **"Edit in designer"**
3. Find the ID field (e.g., `user_id`)
4. Check **"Is Primary Key"** = ✅
5. Save and regenerate

---

## 🔧 **Issue 10: Compilation Errors After Generation**

**Problem:** Generated entities don't compile

**Common Causes:**

### **Missing CommonEntityBase:**
**Error:** `CommonEntityBase` could not be found

**Fix:** Create base class or change to `EntityBase2`:
```csharp
// Project Settings → Entity Model → Entity Base Class
// Change to: EntityBase2 (built-in)
```

### **Wrong .NET Version:**
**Error:** Features not available in .NET X.X

**Fix:**
1. Project Settings → **".NET Framework version"**
2. Set to match ECommerce.Data: **.NET 8.0** or **.NET 10.0**

---

## 📋 **Complete Setup Checklist**

Use this to verify your configuration:

### **1. New Project Dialog:**
- ✅ Name: `ECommerceProject`
- ✅ Location: `D:\source\ecommerce-multiagent-project\backend\ECommerce.Data`
- ✅ Template Group: **Adapter**
- ✅ Template Set: **PostgreSQL**
- ✅ Root Namespace: `ECommerce.Data`

### **2. Database Connection:**
- ✅ Driver: PostgreSQL
- ✅ Server: localhost
- ✅ Port: 5432
- ✅ Database: ecommerce
- ✅ Connection test passes

### **3. Tables Retrieved:**
- ✅ All 16 tables visible in Catalog Explorer
- ✅ Tables **added to project** (visible in Project Explorer)
- ✅ Primary keys detected (check with gold key icon)
- ✅ Foreign keys detected (check relations in Navigator)

### **4. Naming Settings:**
- ✅ Entity pattern: `{$ElementName}Entity`
- ✅ Field casing: PascalCase
- ✅ Remove underscores: YES
- ✅ Test: `users` → `UserEntity`, `user_id` → `UserId`

### **5. Output Settings:**
- ✅ EntityClasses folder configured
- ✅ DatabaseSpecific folder configured
- ✅ Root namespace: `ECommerce.Data`
- ✅ Target .NET: 8.0 or 10.0

### **6. Generation:**
- ✅ Press F5 or Project → Generate
- ✅ Check Output window for errors
- ✅ Verify files created in EntityClasses/
- ✅ Should have 16 *Entity.cs files

### **7. Verification:**
```powershell
# Check entity count
cd D:\source\ecommerce-multiagent-project\backend\ECommerce.Data
Get-ChildItem EntityClasses\*Entity.cs | Measure-Object
# Should show: Count : 16

# Build project
dotnet build
# Should succeed with 0 errors
```

---

## 🎯 **Quick Fixes Summary**

| Issue | Quick Fix |
|-------|-----------|
| Tables not in project | Add to project from Catalog |
| Wrong naming | Settings → Conventions → PascalCase |
| No relations | Select all → Set relationships → Detect |
| Wrong namespace | Settings → Root Namespace → ECommerce.Data |
| Generation fails | Check template = Adapter |
| No files created | Settings → Output folders |
| Compilation errors | Match .NET version |
| UUID issues | Should auto-map to Guid |
| No primary keys | Manually mark in designer |
| Tables not visible | Check schema = public |

---

## 🆘 **Still Having Issues?**

**Share these details:**
1. Screenshot of LLBLGen Designer
2. Exact error message from Output window
3. Which step you're stuck on
4. Contents of Project Explorer panel

I'll help you fix it! 🚀

---

## 📞 **Common Questions:**

**Q: Do I need a license?**
A: LLBLGen Pro requires a license. Trial version works for testing.

**Q: Can I regenerate entities multiple times?**
A: Yes! LLBLGen uses partial classes. Don't modify generated files directly.

**Q: What if I change the database schema?**
A: Refresh catalog → Sync changes → Regenerate (F5)

**Q: Should I commit generated files?**
A: Yes! Commit EntityClasses/ and DatabaseSpecific/ folders.

**Q: What about the .llblgenproj file?**
A: Yes, commit it! Others can regenerate from it.
