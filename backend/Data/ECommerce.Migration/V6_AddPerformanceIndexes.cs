using FluentMigrator;

namespace ECommerce.Migration
{
    /// <summary>
    /// Adds performance indexes and constraints identified during optimization review
    /// </summary>
    [Migration(6, "Add Performance Indexes")]
    public class V6_AddPerformanceIndexes : FluentMigrator.Migration
    {
        public override void Up()
        {
            // Orders: Composite index for customer order history queries
            Execute.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_orders_user_created 
                ON orders(user_id, created_at DESC)
            ");

            // Reviews: Index for approved product reviews sorted by date
            Execute.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_reviews_product_approved_created 
                ON reviews(product_id, is_approved, created_at DESC)
                WHERE is_approved = true
            ");

            // Cart Items: Unique constraint to prevent duplicate items in cart
            Execute.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS uq_cart_items_cart_product 
                ON cart_items(cart_id, product_id)
            ");

            // Payments: Index for payment settlement reports
            Execute.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_payments_status_created 
                ON payments(status, created_at DESC)
            ");

            // Product Images: Index for gallery loading
            Execute.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_product_images_product_order 
                ON product_images(product_id, display_order)
            ");

            // Order Items: Index for fulfillment reports
            Execute.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_order_items_order_created 
                ON order_items(order_id, created_at)
            ");

            // Coupons: Index for active coupon validation
            Execute.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_coupons_valid_active 
                ON coupons(valid_from, valid_until, is_active)
                WHERE is_active = true
            ");

            // Products: Composite index for homepage featured products
            Execute.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_products_featured_active 
                ON products(is_featured, is_active, created_at DESC)
                WHERE is_featured = true AND is_active = true
            ");
        }

        public override void Down()
        {
            Execute.Sql("DROP INDEX IF EXISTS idx_orders_user_created");
            Execute.Sql("DROP INDEX IF EXISTS idx_reviews_product_approved_created");
            Execute.Sql("DROP INDEX IF EXISTS uq_cart_items_cart_product");
            Execute.Sql("DROP INDEX IF EXISTS idx_payments_status_created");
            Execute.Sql("DROP INDEX IF EXISTS idx_product_images_product_order");
            Execute.Sql("DROP INDEX IF EXISTS idx_order_items_order_created");
            Execute.Sql("DROP INDEX IF EXISTS idx_coupons_valid_active");
            Execute.Sql("DROP INDEX IF EXISTS idx_products_featured_active");
        }
    }
}
