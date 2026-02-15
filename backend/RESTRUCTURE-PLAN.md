# E-Commerce Data Layer Restructuring Plan
## Following Mondelez Project Pattern

## 📊 Current Structure (Before)
```
backend/
├── ECommerce.Data/
│   ├── Migrations/
│   ├── Repositories/
│   ├── ECommerce.Data.csproj
│   └── (LLBLGen generated files - mixed in)
└── ECommerce.MigrationRunner/
```

## 🎯 Target Structure (After - Mondelez Pattern)
```
backend/
├── ECommerce.Data/                      ← DatabaseGeneric (Entities)
│   ├── EntityClasses/
│   ├── FactoryClasses/
│   ├── HelperClasses/
│   ├── ConstantsEnums.cs
│   └── ECommerce.Data.csproj
│
├── ECommerce.Data.DatabaseSpecific/     ← Database-specific code
│   ├── DataAccessAdapter.cs
│   ├── PersistenceInfoProvider.cs
│   ├── ActionProcedures.cs
│   ├── RetrievalProcedures.cs
│   └── ECommerce.Data.DatabaseSpecific.csproj
│
├── ECommerce.Data.Migration/            ← Migration runner (already created!)
│   ├── Migrations/
│   ├── Program.cs
│   └── ECommerce.Data.Migration.csproj
│
└── ECommerce.Core/                      ← Business logic + Repositories
    ├── Features/
    │   ├── BaseRepository.cs
    │   ├── Product/
    │   │   ├── IProductRepository.cs
    │   │   ├── ProductRepository.cs
    │   │   └── ProductService.cs
    │   ├── User/
    │   │   ├── IUserRepository.cs
    │   │   ├── UserRepository.cs
    │   │   └── UserService.cs
    │   ├── Cart/
    │   ├── Order/
    │   ├── Review/
    │   └── Category/
    ├── GlobalUsings.cs
    └── ECommerce.Core.csproj
```

## 🔄 Migration Steps

### Step 1: Restructure ECommerce.Data (DatabaseGeneric)
- [x] LLBLGen generated entities already in EntityClasses/
- [ ] Move to separate DatabaseGeneric project
- [ ] Update project to only contain LLBLGen generic code
- [ ] Remove migrations and repositories

### Step 2: Create ECommerce.Data.DatabaseSpecific
- [ ] Create new project for PostgreSQL-specific code
- [ ] Move DataAccessAdapter and related files
- [ ] Reference ECommerce.Data

### Step 3: Rename ECommerce.MigrationRunner
- [ ] Rename to ECommerce.Data.Migration
- [ ] Move migrations from old ECommerce.Data
- [ ] Update references

### Step 4: Create ECommerce.Core
- [ ] Create core business logic project
- [ ] Reference Data and DatabaseSpecific projects
- [ ] Implement Feature-based organization
- [ ] Create GlobalUsings.cs
- [ ] Implement BaseRepository pattern
- [ ] Create repositories by feature

### Step 5: Update ECommerce.API
- [ ] Update references to new project structure
- [ ] Update dependency injection
- [ ] Test API endpoints

## 📦 Project Dependencies

```
ECommerce.API
  └─→ ECommerce.Core
       └─→ ECommerce.Data (DatabaseGeneric)
       └─→ ECommerce.Data.DatabaseSpecific
       
ECommerce.Data.Migration
  └─→ ECommerce.Data
  └─→ ECommerce.Data.DatabaseSpecific

ECommerce.Data.DatabaseSpecific
  └─→ ECommerce.Data
  └─→ SD.LLBLGen.Pro.DQE.PostgreSql

ECommerce.Data
  └─→ SD.LLBLGen.Pro.ORMSupportClasses
```

## 🎨 Feature-Based Organization (ECommerce.Core)

Each feature will have:
```
Features/Product/
  ├── IProductRepository.cs      ← Interface
  ├── ProductRepository.cs       ← Data access
  ├── ProductService.cs          ← Business logic
  ├── ProductValidator.cs        ← Validation
  └── DTOs/
      ├── ProductDto.cs
      └── CreateProductDto.cs
```

## 🔑 Key Patterns from Mondelez

1. **Separation of Concerns:**
   - DatabaseGeneric: Pure entities (no DB logic)
   - DatabaseSpecific: DB connection (PostgreSQL)
   - Core: Business logic + repositories
   - Migration: Schema management

2. **GlobalUsings.cs:**
   ```csharp
   global using Microsoft.Extensions.Localization;
   global using ECommerce.Data.DatabaseSpecific;
   global using ECommerce.Data.EntityClasses;
   global using ECommerce.Data.HelperClasses;
   global using ECommerce.Data.Linq;
   ```

3. **BaseRepository Pattern:**
   ```csharp
   public abstract class BaseRepository
   {
       protected readonly DataAccessAdapter _adapter;
       protected readonly LinqMetaData _meta;
       
       public BaseRepository(DataAccessAdapter adapter)
       {
           _adapter = adapter;
           _meta = new LinqMetaData(_adapter);
       }
   }
   ```

4. **Feature Folders:**
   - Vertical slice architecture
   - All related code in one folder
   - Easy to find and maintain

## 📋 Implementation Checklist

### Phase 1: Project Restructure
- [ ] Create ECommerce.Data.DatabaseSpecific project
- [ ] Move LLBLGen DB-specific files
- [ ] Rename ECommerce.MigrationRunner to ECommerce.Data.Migration
- [ ] Move migrations
- [ ] Update all project references

### Phase 2: Core Business Logic
- [ ] Create ECommerce.Core project
- [ ] Add project references
- [ ] Create GlobalUsings.cs
- [ ] Implement BaseRepository
- [ ] Create feature folders

### Phase 3: Implement Features
- [ ] Product feature (repository + service)
- [ ] User feature
- [ ] Cart feature
- [ ] Order feature
- [ ] Review feature
- [ ] Category feature
- [ ] Coupon feature
- [ ] Wishlist feature
- [ ] Payment feature

### Phase 4: Update API
- [ ] Update project references
- [ ] Update dependency injection
- [ ] Update controllers to use services
- [ ] Test all endpoints

### Phase 5: Testing & Documentation
- [ ] Build all projects
- [ ] Run migrations
- [ ] Test repositories
- [ ] Test services
- [ ] Update documentation

## 🚀 Benefits of This Structure

1. ✅ **Clear Separation:** DB code separate from business logic
2. ✅ **Maintainability:** Easy to find and modify features
3. ✅ **Testability:** Can mock repositories easily
4. ✅ **Scalability:** Add new features without touching existing code
5. ✅ **LLBLGen Friendly:** Generated code isolated in DatabaseGeneric
6. ✅ **Clean Architecture:** Follows industry best practices

## 📝 Notes

- Keep LLBLGen project file (.llblgenproj) in root Data folder
- DatabaseGeneric only references ORMSupportClasses
- DatabaseSpecific references both Generic and DQE.PostgreSql
- Core references both Data projects
- Migration is standalone (only for schema management)

## 🎯 Next Actions

1. Confirm LLBLGen entities are generated correctly
2. Start Phase 1: Project restructure
3. Create new projects
4. Move files systematically
5. Update all references
6. Test builds at each step
