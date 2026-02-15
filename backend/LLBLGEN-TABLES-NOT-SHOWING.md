# LLBLGen Pro - Tables Not Showing Up - SOLUTIONS

## 🔍 **Issue: Tables Don't Show Up in LLBLGen Designer**

Follow these steps **in order** to fix it:

---

## ✅ **Solution 1: Check Schema Setting (MOST COMMON!)**

### **The Problem:**
LLBLGen is looking at the wrong schema. PostgreSQL uses `public` schema by default.

### **The Fix:**

1. In **Catalog Explorer** (left panel), find your database connection
2. **Right-click** on the database node → Select **"Set Schemas to Fetch"**
3. A dialog will appear with schema options
4. **✅ CHECK "public"** (this is where your tables are!)
5. **Uncheck** any other schemas (like `pg_catalog`, `information_schema`)
6. Click **"OK"**
7. **Right-click** on database again → **"Refresh Catalogs"**

**Visual:**
```
┌─────────────────────────────┐
│ Select Schemas to Fetch     │
├─────────────────────────────┤
│ ✅ public                   │
│ ❌ pg_catalog               │
│ ❌ information_schema       │
│ ❌ pg_toast                 │
│                             │
│        [OK]    [Cancel]     │
└─────────────────────────────┘
```

**After clicking OK, you should see:**
```
📁 Catalog Explorer
  └─ 🔌 localhost:5432/ecommerce
      └─ 📁 public ← Should expand to show tables!
          ├─ 📊 addresses
          ├─ 📊 cart_items
          ├─ 📊 carts
          └─ ... (16 tables total)
```

---

## ✅ **Solution 2: Refresh the Catalog**

Sometimes tables are there but catalog is cached.

### **The Fix:**

1. In **Catalog Explorer**, right-click your database
2. Select **"Refresh Catalogs"** or **"Retrieve Catalog Data"**
3. Wait 5-10 seconds for it to load

**Or use full refresh:**
1. **Catalog → Refresh All Catalogs** from menu

---

## ✅ **Solution 3: Verify Database Connection**

### **Test the Connection:**

1. In **Catalog Explorer**, right-click database connection
2. Select **"Edit Connection"**
3. Click **"Test Connection"** button
4. Should say **"Connection successful!"**

**If connection fails:**

### **Fix Connection String:**
```
Driver: PostgreSQL
Server: localhost
Port: 5432
Database: ecommerce
User: postgres
Password: [your password]
```

**Test from command line first:**
```powershell
cd D:\source\ecommerce-multiagent-project\backend
.\test-db-connection.ps1
```

Should show:
```
✅ Connection successful!
📋 Database Statistics:
  Tables: 16
```

If this fails, database connection is the problem!

---

## ✅ **Solution 4: Check Tables Actually Exist**

### **Verify with psql:**

```powershell
# Open PowerShell
$env:PGPASSWORD = "your_password"
& "D:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -d ecommerce -c "\dt"
```

**Expected output:**
```
                List of relations
 Schema |         Name          | Type  |  Owner
--------+-----------------------+-------+----------
 public | addresses             | table | postgres
 public | cart_items            | table | postgres
 public | carts                 | table | postgres
 public | categories            | table | postgres
 public | coupon_usages         | table | postgres
 public | coupons               | table | postgres
 public | order_items           | table | postgres
 public | orders                | table | postgres
 public | payments              | table | postgres
 public | product_images        | table | postgres
 public | products              | table | postgres
 public | reviews               | table | postgres
 public | saved_payment_methods | table | postgres
 public | users                 | table | postgres
 public | wishlist_items        | table | postgres
 public | wishlists             | table | postgres
(16 rows)
```

**If you don't see 16 tables:**
- Migrations didn't run! Go back and run migration runner

---

## ✅ **Solution 5: Database Permissions**

### **Grant Permissions:**

```powershell
$env:PGPASSWORD = "your_password"
& "D:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -d ecommerce -c "GRANT SELECT ON ALL TABLES IN SCHEMA public TO postgres; GRANT USAGE ON SCHEMA public TO postgres;"
```

Then in LLBLGen: **Right-click database → Refresh Catalogs**

---

## ✅ **Solution 6: Re-add Database Connection**

Sometimes the connection gets corrupted. Start fresh:

### **Delete and Re-add:**

1. **Catalog Explorer** → Right-click database → **"Remove Catalog"**
2. Right-click on project root → **"Add New Catalog"**
3. Fill in connection details:
   ```
   Driver Type: PostgreSQL
   Server: localhost
   Port: 5432
   Database: ecommerce
   User: postgres
   Password: ********
   ```
4. Click **"Test Connection"** → Should succeed
5. In advanced options, ensure **Schema: public**
6. Click **"OK"**
7. Right-click new catalog → **"Retrieve Catalog Data"**

---

## ✅ **Solution 7: Check PostgreSQL Driver**

### **Ensure PostgreSQL Driver is Installed:**

LLBLGen needs the Npgsql driver.

1. In LLBLGen, go to **Tools → Preferences**
2. Select **"Database Drivers"**
3. Check that **PostgreSQL** is listed
4. If not, you may need to install/update LLBLGen Pro

---

## 🎯 **Quick Diagnostic Checklist**

Run through this checklist:

```
□ PostgreSQL service is running
  → Get-Service postgresql-x64-17

□ Database 'ecommerce' exists
  → .\test-db-connection.ps1

□ 16 tables exist in database
  → psql -U postgres -d ecommerce -c "\dt"

□ LLBLGen connection test passes
  → Right-click database → Edit → Test Connection

□ Schema 'public' is selected
  → Right-click database → Set Schemas to Fetch → ✅ public

□ Catalog refreshed recently
  → Right-click database → Refresh Catalogs

□ Looking in Catalog Explorer (not Project Explorer)
  → Catalog Explorer should be on LEFT side
```

---

## 📸 **What Should You See?**

### **Correct View:**

```
LLBLGen Pro Designer Window
├─ Menu Bar (File, Edit, Project, ...)
│
├─ Left Panel: CATALOG EXPLORER ← Look here!
│   └─ 🔌 ecommerce@localhost
│       └─ 📁 public
│           ├─ 📊 addresses
│           ├─ 📊 cart_items
│           ├─ 📊 carts
│           ├─ 📊 categories
│           ├─ 📊 coupon_usages
│           ├─ 📊 coupons
│           ├─ 📊 order_items
│           ├─ 📊 orders
│           ├─ 📊 payments
│           ├─ 📊 product_images
│           ├─ 📊 products
│           ├─ 📊 reviews
│           ├─ 📊 saved_payment_methods
│           ├─ 📊 users
│           ├─ 📊 wishlist_items
│           └─ 📊 wishlists
│
└─ Right Panel: PROJECT EXPLORER ← Empty until you add tables!
    └─ (empty - tables need to be added)
```

---

## 🚨 **Common Mistakes**

❌ **Looking in Project Explorer instead of Catalog Explorer**
   → Tables show in CATALOG first, then you add them to PROJECT

❌ **Schema not set to 'public'**
   → 90% of "tables not showing" issues!

❌ **Connection pointing to wrong database**
   → Check connection is to 'ecommerce' not 'postgres'

❌ **Migrations not run yet**
   → Verify with: `.\test-db-connection.ps1`

---

## 🎯 **The Most Likely Fix:**

Based on experience, **90% of the time it's the schema issue**.

**Do this RIGHT NOW:**

1. ✅ Right-click database in Catalog Explorer
2. ✅ Select "Set Schemas to Fetch"
3. ✅ CHECK "public" ← THIS IS THE KEY!
4. ✅ Click OK
5. ✅ Right-click database again
6. ✅ Select "Refresh Catalogs"
7. ✅ Expand the "public" node

**Tables should appear!** 🎉

---

## 📞 **Still Not Working?**

Run this command and share the output:

```powershell
cd D:\source\ecommerce-multiagent-project\backend
.\test-db-connection.ps1
```

Then tell me:
1. What does the test script show?
2. Can you see "public" schema in Catalog Explorer?
3. Is it checked when you go to "Set Schemas to Fetch"?
4. Screenshot of Catalog Explorer panel

I'll help you fix it! 🚀
