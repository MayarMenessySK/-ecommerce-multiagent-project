# Database Engineer Agent - Skill Definition

## Agent Identity
**Role**: Database Engineer  
**Responsibility**: PostgreSQL database design, schema management, Fluent Migrator scripts, LLBLGen Pro coordination  
**Tech Stack**: PostgreSQL 16+, Fluent Migrator, pgAdmin, LLBLGen Pro Designer

## Core Competencies

### 1. PostgreSQL Database Design
- **Schema Design**: Normalized database structures (3NF minimum)
- **Indexing Strategy**: B-tree, Hash, GIN, GiST indexes for performance
- **Constraints**: Primary keys, foreign keys, unique, check constraints
- **Data Types**: Optimal type selection for storage and performance
- **Partitioning**: Table partitioning for large datasets
- **Performance Tuning**: Query optimization, EXPLAIN ANALYZE

### 2. Fluent Migrator Expertise
- **Version-Based Migrations**: Sequential migration scripts
- **Up/Down Methods**: Reversible database changes
- **Seed Data**: Initial data population
- **Complex Migrations**: Data transformations, schema refactoring
- **Migration Testing**: Verify migrations in isolated environments

### 3. LLBLGen Pro Integration
- **Coordinate with Backend Dev**: Ensure schema matches LLBLGen expectations
- **Naming Conventions**: PostgreSQL snake_case to C# PascalCase mapping
- **Entity Relationships**: Proper foreign key setup for LLBLGen navigation
- **Views and Stored Procedures**: Create for complex queries
- **Sync Schema**: Update LLBLGen project after migrations

## Technology Stack

### Tools & Frameworks
- **PostgreSQL 16+**: Primary database
- **Fluent Migrator 5.0**: Database migration framework
- **pgAdmin 4**: Database administration
- **DBeaver**: Alternative database client
- **LLBLGen Pro Designer**: Entity design coordination

### NuGet Packages (in ECommerce.Data project)
```xml
<ItemGroup>
  <PackageReference Include="FluentMigrator" Version="5.0.0" />
  <PackageReference Include="FluentMigrator.Runner" Version="5.0.0" />
  <PackageReference Include="FluentMigrator.Runner.Postgres" Version="5.0.0" />
  <PackageReference Include="Npgsql" Version="8.0.0" />
</ItemGroup>
```

## Database Schema Design

### Naming Conventions
- **Tables**: `lowercase_with_underscores`, plural (e.g., `products`, `order_items`)
- **Columns**: `lowercase_with_underscores` (e.g., `first_name`, `created_at`)
- **Primary Keys**: `id` (simple) or `table_name_id` (explicit)
- **Foreign Keys**: `referenced_table_singular_id` (e.g., `category_id`, `user_id`)
- **Indexes**: `idx_table_column` (e.g., `idx_products_name`)
- **Constraints**: `fk_fromtable_totable`, `chk_table_column`, `uq_table_column`
- **Views**: `table_name_view` or descriptive name with `_view` suffix

### Standard Table Columns
Every table should include:
```sql
id              SERIAL PRIMARY KEY,
created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
updated_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
is_active       BOOLEAN NOT NULL DEFAULT true
```

## E-Commerce Schema Design

### Core Tables

#### 1. Users Table
```sql
CREATE TABLE users (
    id              SERIAL PRIMARY KEY,
    email           VARCHAR(255) NOT NULL UNIQUE,
    password_hash   VARCHAR(500) NOT NULL,
    first_name      VARCHAR(100) NOT NULL,
    last_name       VARCHAR(100) NOT NULL,
    phone           VARCHAR(20),
    role            VARCHAR(50) NOT NULL DEFAULT 'Customer',
    email_verified  BOOLEAN NOT NULL DEFAULT false,
    is_active       BOOLEAN NOT NULL DEFAULT true,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
```

#### 2. Categories Table (Hierarchical)
```sql
CREATE TABLE categories (
    id                  SERIAL PRIMARY KEY,
    name                VARCHAR(100) NOT NULL,
    description         TEXT,
    parent_category_id  INTEGER,
    slug                VARCHAR(150) NOT NULL UNIQUE,
    image_url           VARCHAR(500),
    sort_order          INTEGER DEFAULT 0,
    is_active           BOOLEAN NOT NULL DEFAULT true,
    created_at          TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_categories_parent FOREIGN KEY (parent_category_id) 
        REFERENCES categories(id) ON DELETE SET NULL
);

CREATE INDEX idx_categories_parent ON categories(parent_category_id);
CREATE INDEX idx_categories_slug ON categories(slug);
```

#### 3. Products Table
```sql
CREATE TABLE products (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(200) NOT NULL,
    description     TEXT,
    sku             VARCHAR(100) UNIQUE,
    price           DECIMAL(10, 2) NOT NULL CHECK (price >= 0),
    compare_price   DECIMAL(10, 2) CHECK (compare_price >= price),
    cost_price      DECIMAL(10, 2),
    stock_quantity  INTEGER NOT NULL DEFAULT 0 CHECK (stock_quantity >= 0),
    category_id     INTEGER,
    brand           VARCHAR(100),
    weight          DECIMAL(8, 2),
    dimensions      JSONB,
    image_url       VARCHAR(500),
    images          JSONB,
    meta_title      VARCHAR(200),
    meta_description TEXT,
    is_featured     BOOLEAN NOT NULL DEFAULT false,
    is_active       BOOLEAN NOT NULL DEFAULT true,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_products_category FOREIGN KEY (category_id) 
        REFERENCES categories(id) ON DELETE SET NULL
);

CREATE INDEX idx_products_name ON products(name);
CREATE INDEX idx_products_sku ON products(sku);
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_products_price ON products(price);
CREATE INDEX idx_products_featured ON products(is_featured) WHERE is_featured = true;
```

#### 4. Product Attributes (for variants like color, size)
```sql
CREATE TABLE product_attributes (
    id              SERIAL PRIMARY KEY,
    product_id      INTEGER NOT NULL,
    attribute_name  VARCHAR(50) NOT NULL,
    attribute_value VARCHAR(100) NOT NULL,
    price_modifier  DECIMAL(10, 2) DEFAULT 0,
    
    CONSTRAINT fk_product_attributes_product FOREIGN KEY (product_id) 
        REFERENCES products(id) ON DELETE CASCADE
);

CREATE INDEX idx_product_attributes_product ON product_attributes(product_id);
```

#### 5. Shopping Cart
```sql
CREATE TABLE carts (
    id          SERIAL PRIMARY KEY,
    user_id     INTEGER,
    session_id  VARCHAR(255),
    created_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_carts_user FOREIGN KEY (user_id) 
        REFERENCES users(id) ON DELETE CASCADE
);

CREATE INDEX idx_carts_user ON carts(user_id);
CREATE INDEX idx_carts_session ON carts(session_id);

CREATE TABLE cart_items (
    id          SERIAL PRIMARY KEY,
    cart_id     INTEGER NOT NULL,
    product_id  INTEGER NOT NULL,
    quantity    INTEGER NOT NULL DEFAULT 1 CHECK (quantity > 0),
    price       DECIMAL(10, 2) NOT NULL,
    created_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_cart_items_cart FOREIGN KEY (cart_id) 
        REFERENCES carts(id) ON DELETE CASCADE,
    CONSTRAINT fk_cart_items_product FOREIGN KEY (product_id) 
        REFERENCES products(id) ON DELETE CASCADE,
    CONSTRAINT uq_cart_items_cart_product UNIQUE (cart_id, product_id)
);

CREATE INDEX idx_cart_items_cart ON cart_items(cart_id);
CREATE INDEX idx_cart_items_product ON cart_items(product_id);
```

#### 6. Orders
```sql
CREATE TABLE orders (
    id                  SERIAL PRIMARY KEY,
    order_number        VARCHAR(50) NOT NULL UNIQUE,
    user_id             INTEGER NOT NULL,
    status              VARCHAR(50) NOT NULL DEFAULT 'Pending',
    subtotal            DECIMAL(10, 2) NOT NULL,
    tax_amount          DECIMAL(10, 2) NOT NULL DEFAULT 0,
    shipping_cost       DECIMAL(10, 2) NOT NULL DEFAULT 0,
    discount_amount     DECIMAL(10, 2) NOT NULL DEFAULT 0,
    total_amount        DECIMAL(10, 2) NOT NULL,
    
    -- Shipping Address
    shipping_first_name VARCHAR(100) NOT NULL,
    shipping_last_name  VARCHAR(100) NOT NULL,
    shipping_address1   VARCHAR(255) NOT NULL,
    shipping_address2   VARCHAR(255),
    shipping_city       VARCHAR(100) NOT NULL,
    shipping_state      VARCHAR(100),
    shipping_postal_code VARCHAR(20) NOT NULL,
    shipping_country    VARCHAR(100) NOT NULL,
    shipping_phone      VARCHAR(20),
    
    -- Billing Address (can be same as shipping)
    billing_first_name  VARCHAR(100) NOT NULL,
    billing_last_name   VARCHAR(100) NOT NULL,
    billing_address1    VARCHAR(255) NOT NULL,
    billing_address2    VARCHAR(255),
    billing_city        VARCHAR(100) NOT NULL,
    billing_state       VARCHAR(100),
    billing_postal_code VARCHAR(20) NOT NULL,
    billing_country     VARCHAR(100) NOT NULL,
    
    notes               TEXT,
    created_at          TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_orders_user FOREIGN KEY (user_id) 
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT chk_orders_status CHECK (status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled', 'Refunded'))
);

CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_orders_number ON orders(order_number);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_created ON orders(created_at);

CREATE TABLE order_items (
    id          SERIAL PRIMARY KEY,
    order_id    INTEGER NOT NULL,
    product_id  INTEGER NOT NULL,
    product_name VARCHAR(200) NOT NULL,
    sku         VARCHAR(100),
    quantity    INTEGER NOT NULL CHECK (quantity > 0),
    unit_price  DECIMAL(10, 2) NOT NULL,
    total_price DECIMAL(10, 2) NOT NULL,
    created_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_order_items_order FOREIGN KEY (order_id) 
        REFERENCES orders(id) ON DELETE CASCADE,
    CONSTRAINT fk_order_items_product FOREIGN KEY (product_id) 
        REFERENCES products(id) ON DELETE RESTRICT
);

CREATE INDEX idx_order_items_order ON order_items(order_id);
CREATE INDEX idx_order_items_product ON order_items(product_id);
```

#### 7. Payments
```sql
CREATE TABLE payments (
    id                  SERIAL PRIMARY KEY,
    order_id            INTEGER NOT NULL,
    payment_method      VARCHAR(50) NOT NULL,
    transaction_id      VARCHAR(255),
    stripe_payment_intent_id VARCHAR(255),
    amount              DECIMAL(10, 2) NOT NULL,
    status              VARCHAR(50) NOT NULL DEFAULT 'Pending',
    paid_at             TIMESTAMP,
    created_at          TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_payments_order FOREIGN KEY (order_id) 
        REFERENCES orders(id) ON DELETE RESTRICT,
    CONSTRAINT chk_payments_status CHECK (status IN ('Pending', 'Completed', 'Failed', 'Refunded'))
);

CREATE INDEX idx_payments_order ON payments(order_id);
CREATE INDEX idx_payments_transaction ON payments(transaction_id);
CREATE INDEX idx_payments_stripe ON payments(stripe_payment_intent_id);
```

#### 8. Reviews and Ratings
```sql
CREATE TABLE product_reviews (
    id          SERIAL PRIMARY KEY,
    product_id  INTEGER NOT NULL,
    user_id     INTEGER NOT NULL,
    rating      INTEGER NOT NULL CHECK (rating BETWEEN 1 AND 5),
    title       VARCHAR(200),
    comment     TEXT,
    is_verified_purchase BOOLEAN NOT NULL DEFAULT false,
    is_approved BOOLEAN NOT NULL DEFAULT false,
    created_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_reviews_product FOREIGN KEY (product_id) 
        REFERENCES products(id) ON DELETE CASCADE,
    CONSTRAINT fk_reviews_user FOREIGN KEY (user_id) 
        REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT uq_reviews_user_product UNIQUE (user_id, product_id)
);

CREATE INDEX idx_reviews_product ON product_reviews(product_id);
CREATE INDEX idx_reviews_user ON product_reviews(user_id);
CREATE INDEX idx_reviews_approved ON product_reviews(is_approved) WHERE is_approved = true;
```

#### 9. Discounts and Coupons
```sql
CREATE TABLE coupons (
    id              SERIAL PRIMARY KEY,
    code            VARCHAR(50) NOT NULL UNIQUE,
    description     TEXT,
    discount_type   VARCHAR(20) NOT NULL,
    discount_value  DECIMAL(10, 2) NOT NULL,
    min_order_amount DECIMAL(10, 2),
    max_uses        INTEGER,
    used_count      INTEGER NOT NULL DEFAULT 0,
    valid_from      TIMESTAMP NOT NULL,
    valid_until     TIMESTAMP NOT NULL,
    is_active       BOOLEAN NOT NULL DEFAULT true,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT chk_coupons_discount_type CHECK (discount_type IN ('Percentage', 'Fixed'))
);

CREATE INDEX idx_coupons_code ON coupons(code);
CREATE INDEX idx_coupons_active ON coupons(is_active) WHERE is_active = true;
```

### Database Views

#### Product Inventory View
```sql
CREATE OR REPLACE VIEW product_inventory_view AS
SELECT 
    p.id,
    p.name,
    p.sku,
    p.price,
    p.stock_quantity,
    c.name AS category_name,
    CASE 
        WHEN p.stock_quantity = 0 THEN 'Out of Stock'
        WHEN p.stock_quantity < 10 THEN 'Low Stock'
        ELSE 'In Stock'
    END AS stock_status,
    p.is_active
FROM products p
LEFT JOIN categories c ON p.category_id = c.id;
```

#### Order Summary View
```sql
CREATE OR REPLACE VIEW order_summary_view AS
SELECT 
    o.id,
    o.order_number,
    o.user_id,
    u.email AS user_email,
    u.first_name || ' ' || u.last_name AS customer_name,
    o.status,
    o.total_amount,
    COUNT(oi.id) AS total_items,
    SUM(oi.quantity) AS total_quantity,
    o.created_at,
    p.status AS payment_status
FROM orders o
INNER JOIN users u ON o.user_id = u.id
LEFT JOIN order_items oi ON o.id = oi.order_id
LEFT JOIN payments p ON o.id = p.order_id
GROUP BY o.id, o.order_number, o.user_id, u.email, u.first_name, u.last_name, 
         o.status, o.total_amount, o.created_at, p.status;
```

## Fluent Migrator Implementation

### Migration Template
```csharp
using FluentMigrator;

namespace ECommerce.Data.Migrations;

[Migration(XXXX, "Description of what this migration does")]
public class VXXXX_MigrationName : Migration
{
    public override void Up()
    {
        // Create tables, add columns, create indexes, etc.
    }

    public override void Down()
    {
        // Reverse the changes made in Up()
    }
}
```

### Example: Complete Migration
```csharp
using FluentMigrator;

namespace ECommerce.Data.Migrations;

[Migration(1, "Create initial e-commerce tables")]
public class V1_CreateInitialTables : Migration
{
    public override void Up()
    {
        // Users table
        Create.Table("users")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("email").AsString(255).NotNullable().Unique()
            .WithColumn("password_hash").AsString(500).NotNullable()
            .WithColumn("first_name").AsString(100).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("phone").AsString(20).Nullable()
            .WithColumn("role").AsString(50).NotNullable().WithDefaultValue("Customer")
            .WithColumn("email_verified").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Index("idx_users_email").OnTable("users").OnColumn("email");
        Create.Index("idx_users_role").OnTable("users").OnColumn("role");

        // Categories table
        Create.Table("categories")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("description").AsString(2000).Nullable()
            .WithColumn("parent_category_id").AsInt32().Nullable()
            .WithColumn("slug").AsString(150).NotNullable().Unique()
            .WithColumn("image_url").AsString(500).Nullable()
            .WithColumn("sort_order").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.ForeignKey("fk_categories_parent")
            .FromTable("categories").ForeignColumn("parent_category_id")
            .ToTable("categories").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.SetNull);

        Create.Index("idx_categories_parent").OnTable("categories").OnColumn("parent_category_id");
        Create.Index("idx_categories_slug").OnTable("categories").OnColumn("slug");

        // Products table
        Create.Table("products")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(5000).Nullable()
            .WithColumn("sku").AsString(100).Nullable().Unique()
            .WithColumn("price").AsDecimal(10, 2).NotNullable()
            .WithColumn("compare_price").AsDecimal(10, 2).Nullable()
            .WithColumn("cost_price").AsDecimal(10, 2).Nullable()
            .WithColumn("stock_quantity").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("category_id").AsInt32().Nullable()
            .WithColumn("brand").AsString(100).Nullable()
            .WithColumn("weight").AsDecimal(8, 2).Nullable()
            .WithColumn("dimensions").AsCustom("JSONB").Nullable()
            .WithColumn("image_url").AsString(500).Nullable()
            .WithColumn("images").AsCustom("JSONB").Nullable()
            .WithColumn("meta_title").AsString(200).Nullable()
            .WithColumn("meta_description").AsString(2000).Nullable()
            .WithColumn("is_featured").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.ForeignKey("fk_products_category")
            .FromTable("products").ForeignColumn("category_id")
            .ToTable("categories").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.SetNull);

        Create.Index("idx_products_name").OnTable("products").OnColumn("name");
        Create.Index("idx_products_sku").OnTable("products").OnColumn("sku");
        Create.Index("idx_products_category").OnTable("products").OnColumn("category_id");
        Create.Index("idx_products_price").OnTable("products").OnColumn("price");
    }

    public override void Down()
    {
        Delete.Table("products");
        Delete.Table("categories");
        Delete.Table("users");
    }
}
```

### Seed Data Migration
```csharp
[Migration(100, "Seed initial data")]
public class V100_SeedInitialData : Migration
{
    public override void Up()
    {
        // Seed admin user
        Insert.IntoTable("users").Row(new
        {
            email = "admin@ecommerce.com",
            password_hash = "$2a$11$hashed_password_here", // Use BCrypt
            first_name = "Admin",
            last_name = "User",
            role = "SuperAdmin",
            email_verified = true,
            is_active = true,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        });

        // Seed categories
        Insert.IntoTable("categories").Row(new
        {
            name = "Electronics",
            slug = "electronics",
            description = "Electronic devices and accessories",
            is_active = true,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        });

        Insert.IntoTable("categories").Row(new
        {
            name = "Clothing",
            slug = "clothing",
            description = "Men's and women's clothing",
            is_active = true,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        });
    }

    public override void Down()
    {
        Delete.FromTable("categories").AllRows();
        Delete.FromTable("users").AllRows();
    }
}
```

## LLBLGen Pro Coordination

### 1. Workflow with Backend Developer
1. **Database Engineer**: Create migration, apply to dev database
2. **Database Engineer**: Refresh LLBLGen Pro project from database
3. **Database Engineer**: Generate entities, commit to Git
4. **Backend Developer**: Pull latest entities, implement repositories

### 2. Entity Naming Mapping
PostgreSQL → LLBLGen Pro:
- `product_categories` → `ProductCategoryEntity`
- `order_items` → `OrderItemEntity`
- `shopping_carts` → `ShoppingCartEntity`

### 3. LLBLGen Pro Project Settings
```
- Database: PostgreSQL
- Template: Adapter
- Entity base class: CommonEntityBase
- Field naming: PascalCase
- Table naming: Unchanged (snake_case)
- Generate TypedViews for all views
```

## Performance Optimization

### Indexing Strategy
1. **Primary Keys**: Automatically indexed
2. **Foreign Keys**: Always index (e.g., `category_id`, `user_id`)
3. **Search Columns**: Index columns used in WHERE clauses (`name`, `email`, `sku`)
4. **Sort Columns**: Index columns used in ORDER BY
5. **Composite Indexes**: For multi-column queries
6. **Partial Indexes**: For filtered queries (e.g., `WHERE is_active = true`)

### Query Optimization
```sql
-- Use EXPLAIN ANALYZE for query performance
EXPLAIN ANALYZE
SELECT p.*, c.name AS category_name
FROM products p
LEFT JOIN categories c ON p.category_id = c.id
WHERE p.is_active = true
  AND p.price BETWEEN 10 AND 100
ORDER BY p.created_at DESC
LIMIT 20;
```

## Database Maintenance

### Backup Strategy
```bash
# Daily automated backup
pg_dump -h localhost -U postgres -d ecommerce -F c -f backup_$(date +%Y%m%d).dump

# Restore from backup
pg_restore -h localhost -U postgres -d ecommerce -c backup_20260211.dump
```

### Vacuum and Analyze
```sql
-- Regular maintenance
VACUUM ANALYZE products;
VACUUM ANALYZE orders;

-- Full vacuum (less frequently)
VACUUM FULL;
```

## Git Workflow
- **Branch Naming**: `feature/database/[feature-name]`
- **Migration Naming**: `VXXX_DescriptiveName.cs` (sequential numbers)
- **Commit Messages**: Include migration version number
- **Pull Requests**: Include database schema changes, migration scripts

## Testing Migrations

### Local Testing
1. Create clean test database
2. Run all migrations from scratch
3. Verify schema correctness
4. Test rollback (Down methods)
5. Test seed data

### Migration Validation
```bash
# Run migrations
dotnet run --project ECommerce.API -- migrate up

# Rollback last migration
dotnet run --project ECommerce.API -- migrate down --steps 1

# Migrate to specific version
dotnet run --project ECommerce.API -- migrate up --version 5
```

## Deliverables
- [ ] Complete database schema design
- [ ] All Fluent Migrator scripts (sequential, version-based)
- [ ] Database views for complex queries
- [ ] Seed data migrations
- [ ] LLBLGen Pro project configuration
- [ ] Entity generation and coordination with Backend Dev
- [ ] Database indexes for performance
- [ ] Database documentation (ERD, table descriptions)
- [ ] Backup and restore procedures
- [ ] Performance tuning guidelines

## Success Criteria
- All migrations execute successfully without errors
- Database schema is normalized (3NF)
- Foreign key relationships are correct
- Indexes are optimized for query performance
- LLBLGen Pro entities generate correctly
- Seed data populates successfully
- Backup/restore procedures tested
- Database handles expected load (performance tested)
