using FluentMigrator;

namespace ECommerce.Migration;

[Migration(1)]
public class V1_InitialSchema : FluentMigrator.Migration
{
    public override void Up()
    {
        // Users table
        Create.Table("users")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("email").AsString(255).NotNullable().Unique()
            .WithColumn("password_hash").AsString(500).NotNullable()
            .WithColumn("first_name").AsString(100).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("phone_number").AsString(20).Nullable()
            .WithColumn("role").AsString(50).NotNullable().WithDefaultValue("Customer")
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_email_verified").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("email_verification_token").AsString(500).Nullable()
            .WithColumn("password_reset_token").AsString(500).Nullable()
            .WithColumn("password_reset_expires").AsDateTime().Nullable()
            .WithColumn("failed_login_attempts").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("lockout_end").AsDateTime().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("last_login_at").AsDateTime().Nullable();

        Create.Index("ix_users_email").OnTable("users").OnColumn("email");
        Create.Index("ix_users_role").OnTable("users").OnColumn("role");

        // Addresses table
        Create.Table("addresses")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("full_name").AsString(200).NotNullable()
            .WithColumn("phone_number").AsString(20).NotNullable()
            .WithColumn("address_line1").AsString(255).NotNullable()
            .WithColumn("address_line2").AsString(255).Nullable()
            .WithColumn("city").AsString(100).NotNullable()
            .WithColumn("state").AsString(100).NotNullable()
            .WithColumn("postal_code").AsString(20).NotNullable()
            .WithColumn("country").AsString(100).NotNullable()
            .WithColumn("is_default").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("address_type").AsString(20).NotNullable().WithDefaultValue("Shipping")
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_addresses_users")
            .FromTable("addresses").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_addresses_user_id").OnTable("addresses").OnColumn("user_id");

        // Categories table
        Create.Table("categories")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("slug").AsString(100).NotNullable().Unique()
            .WithColumn("description").AsString(1000).Nullable()
            .WithColumn("image_url").AsString(500).Nullable()
            .WithColumn("parent_category_id").AsGuid().Nullable()
            .WithColumn("level").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("display_order").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_categories_parent")
            .FromTable("categories").ForeignColumn("parent_category_id")
            .ToTable("categories").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.SetNull);

        Create.Index("ix_categories_slug").OnTable("categories").OnColumn("slug");
        Create.Index("ix_categories_parent_id").OnTable("categories").OnColumn("parent_category_id");

        // Products table
        Create.Table("products")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("slug").AsString(255).NotNullable().Unique()
            .WithColumn("sku").AsString(100).NotNullable().Unique()
            .WithColumn("description").AsString(5000).Nullable()
            .WithColumn("short_description").AsString(500).Nullable()
            .WithColumn("price").AsDecimal(10, 2).NotNullable()
            .WithColumn("original_price").AsDecimal(10, 2).Nullable()
            .WithColumn("discount_percentage").AsDecimal(5, 2).Nullable()
            .WithColumn("currency").AsString(3).NotNullable().WithDefaultValue("USD")
            .WithColumn("stock_quantity").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("low_stock_threshold").AsInt32().NotNullable().WithDefaultValue(10)
            .WithColumn("category_id").AsGuid().NotNullable()
            .WithColumn("brand").AsString(100).Nullable()
            .WithColumn("weight").AsDecimal(10, 2).Nullable()
            .WithColumn("dimensions").AsString(100).Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_featured").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("average_rating").AsDecimal(3, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("total_reviews").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("total_sales").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("meta_title").AsString(255).Nullable()
            .WithColumn("meta_description").AsString(500).Nullable()
            .WithColumn("meta_keywords").AsString(500).Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_products_categories")
            .FromTable("products").ForeignColumn("category_id")
            .ToTable("categories").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.None);

        Create.Index("ix_products_slug").OnTable("products").OnColumn("slug");
        Create.Index("ix_products_sku").OnTable("products").OnColumn("sku");
        Create.Index("ix_products_category_id").OnTable("products").OnColumn("category_id");
        Create.Index("ix_products_price").OnTable("products").OnColumn("price");
        Create.Index("ix_products_is_active").OnTable("products").OnColumn("is_active");
        Create.Index("ix_products_is_featured").OnTable("products").OnColumn("is_featured");

        // Product Images table
        Create.Table("product_images")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("image_url").AsString(500).NotNullable()
            .WithColumn("alt_text").AsString(255).Nullable()
            .WithColumn("is_primary").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("display_order").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_product_images_products")
            .FromTable("product_images").ForeignColumn("product_id")
            .ToTable("products").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_product_images_product_id").OnTable("product_images").OnColumn("product_id");

        // Carts table
        Create.Table("carts")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("user_id").AsGuid().Nullable()
            .WithColumn("session_id").AsString(100).Nullable()
            .WithColumn("subtotal").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("tax").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("shipping").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("discount").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("total").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("discount_code").AsString(50).Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_carts_users")
            .FromTable("carts").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_carts_user_id").OnTable("carts").OnColumn("user_id");
        Create.Index("ix_carts_session_id").OnTable("carts").OnColumn("session_id");

        // Cart Items table
        Create.Table("cart_items")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("cart_id").AsGuid().NotNullable()
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("quantity").AsInt32().NotNullable().WithDefaultValue(1)
            .WithColumn("price").AsDecimal(10, 2).NotNullable()
            .WithColumn("subtotal").AsDecimal(10, 2).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_cart_items_carts")
            .FromTable("cart_items").ForeignColumn("cart_id")
            .ToTable("carts").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_cart_items_products")
            .FromTable("cart_items").ForeignColumn("product_id")
            .ToTable("products").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.None);

        Create.Index("ix_cart_items_cart_id").OnTable("cart_items").OnColumn("cart_id");
        Create.Index("ix_cart_items_product_id").OnTable("cart_items").OnColumn("product_id");

        // Orders table
        Create.Table("orders")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("order_number").AsString(50).NotNullable().Unique()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("status").AsString(50).NotNullable().WithDefaultValue("Pending")
            .WithColumn("subtotal").AsDecimal(10, 2).NotNullable()
            .WithColumn("tax").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("shipping_cost").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("discount").AsDecimal(10, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("total").AsDecimal(10, 2).NotNullable()
            .WithColumn("currency").AsString(3).NotNullable().WithDefaultValue("USD")
            .WithColumn("discount_code").AsString(50).Nullable()
            .WithColumn("payment_method").AsString(50).NotNullable()
            .WithColumn("payment_status").AsString(50).NotNullable().WithDefaultValue("Pending")
            .WithColumn("payment_transaction_id").AsString(255).Nullable()
            .WithColumn("shipping_method").AsString(50).NotNullable()
            .WithColumn("tracking_number").AsString(100).Nullable()
            .WithColumn("shipping_carrier").AsString(100).Nullable()
            .WithColumn("shipping_full_name").AsString(200).NotNullable()
            .WithColumn("shipping_phone").AsString(20).NotNullable()
            .WithColumn("shipping_address_line1").AsString(255).NotNullable()
            .WithColumn("shipping_address_line2").AsString(255).Nullable()
            .WithColumn("shipping_city").AsString(100).NotNullable()
            .WithColumn("shipping_state").AsString(100).NotNullable()
            .WithColumn("shipping_postal_code").AsString(20).NotNullable()
            .WithColumn("shipping_country").AsString(100).NotNullable()
            .WithColumn("billing_full_name").AsString(200).Nullable()
            .WithColumn("billing_phone").AsString(20).Nullable()
            .WithColumn("billing_address_line1").AsString(255).Nullable()
            .WithColumn("billing_address_line2").AsString(255).Nullable()
            .WithColumn("billing_city").AsString(100).Nullable()
            .WithColumn("billing_state").AsString(100).Nullable()
            .WithColumn("billing_postal_code").AsString(20).Nullable()
            .WithColumn("billing_country").AsString(100).Nullable()
            .WithColumn("notes").AsString(1000).Nullable()
            .WithColumn("estimated_delivery_date").AsDateTime().Nullable()
            .WithColumn("delivered_at").AsDateTime().Nullable()
            .WithColumn("cancelled_at").AsDateTime().Nullable()
            .WithColumn("cancellation_reason").AsString(500).Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_orders_users")
            .FromTable("orders").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.None);

        Create.Index("ix_orders_order_number").OnTable("orders").OnColumn("order_number");
        Create.Index("ix_orders_user_id").OnTable("orders").OnColumn("user_id");
        Create.Index("ix_orders_status").OnTable("orders").OnColumn("status");
        Create.Index("ix_orders_created_at").OnTable("orders").OnColumn("created_at");

        // Order Items table
        Create.Table("order_items")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("order_id").AsGuid().NotNullable()
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("product_name").AsString(255).NotNullable()
            .WithColumn("product_sku").AsString(100).NotNullable()
            .WithColumn("product_image_url").AsString(500).Nullable()
            .WithColumn("quantity").AsInt32().NotNullable()
            .WithColumn("price").AsDecimal(10, 2).NotNullable()
            .WithColumn("subtotal").AsDecimal(10, 2).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_order_items_orders")
            .FromTable("order_items").ForeignColumn("order_id")
            .ToTable("orders").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_order_items_products")
            .FromTable("order_items").ForeignColumn("product_id")
            .ToTable("products").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.None);

        Create.Index("ix_order_items_order_id").OnTable("order_items").OnColumn("order_id");
        Create.Index("ix_order_items_product_id").OnTable("order_items").OnColumn("product_id");

        // Reviews table
        Create.Table("reviews")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("order_id").AsGuid().Nullable()
            .WithColumn("rating").AsInt32().NotNullable()
            .WithColumn("title").AsString(200).Nullable()
            .WithColumn("comment").AsString(2000).Nullable()
            .WithColumn("is_verified_purchase").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("is_approved").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("helpful_count").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("unhelpful_count").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_reviews_products")
            .FromTable("reviews").ForeignColumn("product_id")
            .ToTable("products").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_reviews_users")
            .FromTable("reviews").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_reviews_orders")
            .FromTable("reviews").ForeignColumn("order_id")
            .ToTable("orders").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.SetNull);

        Create.Index("ix_reviews_product_id").OnTable("reviews").OnColumn("product_id");
        Create.Index("ix_reviews_user_id").OnTable("reviews").OnColumn("user_id");
        Create.Index("ix_reviews_rating").OnTable("reviews").OnColumn("rating");
    }

    public override void Down()
    {
        Delete.Table("reviews");
        Delete.Table("order_items");
        Delete.Table("orders");
        Delete.Table("cart_items");
        Delete.Table("carts");
        Delete.Table("product_images");
        Delete.Table("products");
        Delete.Table("categories");
        Delete.Table("addresses");
        Delete.Table("users");
    }
}

