using FluentMigrator;

namespace ECommerce.Migration;

[Migration(5, "Create database views for performance and reporting")]
public class V5_CreateViews : FluentMigrator.Migration
{
    public override void Up()
    {
        // Product Inventory View - Shows stock status with category info
        Execute.Sql(@"
            CREATE OR REPLACE VIEW product_inventory_view AS
            SELECT 
                p.id,
                p.name,
                p.sku,
                p.price,
                p.original_price,
                p.discount_percentage,
                p.stock_quantity,
                p.low_stock_threshold,
                c.name AS category_name,
                c.slug AS category_slug,
                CASE 
                    WHEN p.stock_quantity = 0 THEN 'Out of Stock'
                    WHEN p.stock_quantity <= p.low_stock_threshold THEN 'Low Stock'
                    ELSE 'In Stock'
                END AS stock_status,
                p.is_active,
                p.is_featured,
                p.average_rating,
                p.total_reviews,
                p.total_sales,
                p.created_at,
                p.updated_at
            FROM products p
            LEFT JOIN categories c ON p.category_id = c.id
            WHERE p.is_active = true;
        ");

        // Product Rating View - Aggregated rating statistics
        Execute.Sql(@"
            CREATE OR REPLACE VIEW product_rating_view AS
            SELECT 
                p.id AS product_id,
                p.name AS product_name,
                p.average_rating,
                p.total_reviews,
                COUNT(CASE WHEN r.rating = 5 THEN 1 END) AS five_star_count,
                COUNT(CASE WHEN r.rating = 4 THEN 1 END) AS four_star_count,
                COUNT(CASE WHEN r.rating = 3 THEN 1 END) AS three_star_count,
                COUNT(CASE WHEN r.rating = 2 THEN 1 END) AS two_star_count,
                COUNT(CASE WHEN r.rating = 1 THEN 1 END) AS one_star_count,
                ROUND(AVG(r.rating)::numeric, 2) AS calculated_avg_rating,
                COUNT(CASE WHEN r.is_verified_purchase = true THEN 1 END) AS verified_purchase_count
            FROM products p
            LEFT JOIN reviews r ON p.id = r.product_id AND r.is_approved = true
            GROUP BY p.id, p.name, p.average_rating, p.total_reviews;
        ");

        // Order Summary View - Complete order details with customer info
        Execute.Sql(@"
            CREATE OR REPLACE VIEW order_summary_view AS
            SELECT 
                o.id,
                o.order_number,
                o.user_id,
                u.email AS user_email,
                u.first_name || ' ' || u.last_name AS customer_name,
                o.status,
                o.payment_status,
                o.payment_method,
                o.subtotal,
                o.tax,
                o.shipping_cost,
                o.discount,
                o.total,
                o.currency,
                COUNT(oi.id) AS total_items,
                SUM(oi.quantity) AS total_quantity,
                o.shipping_full_name,
                o.shipping_city,
                o.shipping_state,
                o.shipping_country,
                o.tracking_number,
                o.shipping_carrier,
                o.estimated_delivery_date,
                o.delivered_at,
                o.created_at,
                o.updated_at
            FROM orders o
            INNER JOIN users u ON o.user_id = u.id
            LEFT JOIN order_items oi ON o.id = oi.order_id
            GROUP BY o.id, o.order_number, o.user_id, u.email, u.first_name, u.last_name, 
                     o.status, o.payment_status, o.payment_method, o.subtotal, o.tax, 
                     o.shipping_cost, o.discount, o.total, o.currency, o.shipping_full_name,
                     o.shipping_city, o.shipping_state, o.shipping_country, o.tracking_number,
                     o.shipping_carrier, o.estimated_delivery_date, o.delivered_at, 
                     o.created_at, o.updated_at;
        ");

        // Customer Orders View - Customer purchase history with statistics
        Execute.Sql(@"
            CREATE OR REPLACE VIEW customer_orders_view AS
            SELECT 
                u.id AS user_id,
                u.email,
                u.first_name,
                u.last_name,
                COUNT(DISTINCT o.id) AS total_orders,
                SUM(o.total) AS lifetime_value,
                AVG(o.total) AS average_order_value,
                MAX(o.created_at) AS last_order_date,
                MIN(o.created_at) AS first_order_date,
                COUNT(CASE WHEN o.status = 'Delivered' THEN 1 END) AS delivered_orders,
                COUNT(CASE WHEN o.status = 'Cancelled' THEN 1 END) AS cancelled_orders,
                u.created_at AS customer_since
            FROM users u
            LEFT JOIN orders o ON u.id = o.user_id
            WHERE u.role = 'Customer'
            GROUP BY u.id, u.email, u.first_name, u.last_name, u.created_at;
        ");

        // Cart Summary View - Active carts with item details
        Execute.Sql(@"
            CREATE OR REPLACE VIEW cart_summary_view AS
            SELECT 
                c.id AS cart_id,
                c.user_id,
                u.email AS user_email,
                c.session_id,
                COUNT(ci.id) AS total_items,
                SUM(ci.quantity) AS total_quantity,
                c.subtotal,
                c.tax,
                c.shipping,
                c.discount,
                c.total,
                c.discount_code,
                c.created_at,
                c.updated_at,
                EXTRACT(EPOCH FROM (NOW() - c.updated_at))/3600 AS hours_since_update
            FROM carts c
            LEFT JOIN users u ON c.user_id = u.id
            LEFT JOIN cart_items ci ON c.id = ci.cart_id
            GROUP BY c.id, c.user_id, u.email, c.session_id, c.subtotal, c.tax, 
                     c.shipping, c.discount, c.total, c.discount_code, 
                     c.created_at, c.updated_at;
        ");

        // Sales Performance View - Daily/weekly/monthly sales statistics
        Execute.Sql(@"
            CREATE OR REPLACE VIEW sales_performance_view AS
            SELECT 
                DATE_TRUNC('day', o.created_at) AS sale_date,
                COUNT(DISTINCT o.id) AS orders_count,
                SUM(o.total) AS total_revenue,
                SUM(o.subtotal) AS total_subtotal,
                SUM(o.tax) AS total_tax,
                SUM(o.shipping_cost) AS total_shipping,
                SUM(o.discount) AS total_discounts,
                AVG(o.total) AS average_order_value,
                COUNT(DISTINCT o.user_id) AS unique_customers,
                SUM(oi.quantity) AS total_items_sold
            FROM orders o
            LEFT JOIN order_items oi ON o.id = oi.order_id
            WHERE o.status NOT IN ('Cancelled')
            GROUP BY DATE_TRUNC('day', o.created_at)
            ORDER BY sale_date DESC;
        ");

        // Category Performance View - Sales by category
        Execute.Sql(@"
            CREATE OR REPLACE VIEW category_performance_view AS
            SELECT 
                c.id AS category_id,
                c.name AS category_name,
                c.slug AS category_slug,
                COUNT(DISTINCT p.id) AS total_products,
                COUNT(DISTINCT CASE WHEN p.is_active = true THEN p.id END) AS active_products,
                SUM(p.total_sales) AS total_sales,
                AVG(p.average_rating) AS average_category_rating,
                SUM(p.stock_quantity) AS total_stock,
                MIN(p.price) AS min_price,
                MAX(p.price) AS max_price,
                AVG(p.price) AS avg_price
            FROM categories c
            LEFT JOIN products p ON c.id = p.category_id
            GROUP BY c.id, c.name, c.slug;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP VIEW IF EXISTS category_performance_view;");
        Execute.Sql("DROP VIEW IF EXISTS sales_performance_view;");
        Execute.Sql("DROP VIEW IF EXISTS cart_summary_view;");
        Execute.Sql("DROP VIEW IF EXISTS customer_orders_view;");
        Execute.Sql("DROP VIEW IF EXISTS order_summary_view;");
        Execute.Sql("DROP VIEW IF EXISTS product_rating_view;");
        Execute.Sql("DROP VIEW IF EXISTS product_inventory_view;");
    }
}

