using FluentMigrator;

namespace ECommerce.Migration;

[Migration(100, "Seed initial data for testing and development")]
public class V100_SeedInitialData : FluentMigrator.Migration
{
    public override void Up()
    {
        // Seed Admin User (password: Admin@123)
        // BCrypt hash for "Admin@123"
        Execute.Sql(@"
            INSERT INTO users (id, email, password_hash, first_name, last_name, phone_number, role, is_active, is_email_verified, created_at, updated_at)
            VALUES (
                gen_random_uuid(),
                'admin@ecommerce.com',
                '$2a$11$YourBCryptHashHere', -- Replace with actual BCrypt hash
                'Admin',
                'User',
                '+1234567890',
                'SuperAdmin',
                true,
                true,
                CURRENT_TIMESTAMP,
                CURRENT_TIMESTAMP
            );
        ");

        // Seed Test Customer User (password: Customer@123)
        Execute.Sql(@"
            INSERT INTO users (id, email, password_hash, first_name, last_name, phone_number, role, is_active, is_email_verified, created_at, updated_at)
            VALUES (
                gen_random_uuid(),
                'customer@test.com',
                '$2a$11$YourBCryptHashHere', -- Replace with actual BCrypt hash
                'John',
                'Doe',
                '+1234567891',
                'Customer',
                true,
                true,
                CURRENT_TIMESTAMP,
                CURRENT_TIMESTAMP
            );
        ");

        // Seed Categories
        Execute.Sql(@"
            INSERT INTO categories (id, name, slug, description, image_url, parent_category_id, level, is_active, display_order, created_at, updated_at)
            VALUES 
            (gen_random_uuid(), 'Electronics', 'electronics', 'Electronic devices and accessories', 'https://via.placeholder.com/300x200?text=Electronics', NULL, 0, true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'Clothing', 'clothing', 'Mens and womens clothing', 'https://via.placeholder.com/300x200?text=Clothing', NULL, 0, true, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'Books', 'books', 'Books and educational materials', 'https://via.placeholder.com/300x200?text=Books', NULL, 0, true, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'Home & Garden', 'home-garden', 'Home and garden products', 'https://via.placeholder.com/300x200?text=Home+Garden', NULL, 0, true, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'Sports & Outdoors', 'sports-outdoors', 'Sports equipment and outdoor gear', 'https://via.placeholder.com/300x200?text=Sports', NULL, 0, true, 5, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'Toys & Games', 'toys-games', 'Toys and games for all ages', 'https://via.placeholder.com/300x200?text=Toys', NULL, 0, true, 6, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'Beauty & Personal Care', 'beauty-personal-care', 'Beauty and personal care products', 'https://via.placeholder.com/300x200?text=Beauty', NULL, 0, true, 7, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'Automotive', 'automotive', 'Automotive parts and accessories', 'https://via.placeholder.com/300x200?text=Automotive', NULL, 0, true, 8, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
        ");

        // Get Electronics category ID for products
        Execute.Sql(@"
            DO $$
            DECLARE
                electronics_id UUID;
                clothing_id UUID;
                books_id UUID;
            BEGIN
                SELECT id INTO electronics_id FROM categories WHERE slug = 'electronics' LIMIT 1;
                SELECT id INTO clothing_id FROM categories WHERE slug = 'clothing' LIMIT 1;
                SELECT id INTO books_id FROM categories WHERE slug = 'books' LIMIT 1;

                -- Seed Sample Products - Electronics
                INSERT INTO products (id, name, slug, sku, description, short_description, price, original_price, discount_percentage, stock_quantity, low_stock_threshold, category_id, brand, is_active, is_featured, average_rating, total_reviews, created_at, updated_at)
                VALUES 
                (gen_random_uuid(), 'Wireless Bluetooth Headphones', 'wireless-bluetooth-headphones', 'WBH-001', 'High-quality wireless headphones with noise cancellation and 30-hour battery life. Perfect for music lovers and professionals.', 'Premium wireless headphones with noise cancellation', 299.99, 399.99, 25.00, 50, 10, electronics_id, 'AudioTech', true, true, 4.5, 127, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                (gen_random_uuid(), 'Smartwatch Pro X', 'smartwatch-pro-x', 'SWP-X-001', 'Advanced smartwatch with fitness tracking, heart rate monitoring, GPS, and 7-day battery life. Water-resistant up to 50m.', 'Advanced fitness and health smartwatch', 449.99, 599.99, 25.00, 30, 5, electronics_id, 'TechWear', true, true, 4.7, 89, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                (gen_random_uuid(), 'Laptop Ultrabook 15 inch', 'laptop-ultrabook-15', 'LU-15-001', 'Powerful ultrabook with Intel i7, 16GB RAM, 512GB SSD, and stunning 4K display. Perfect for work and entertainment.', 'Ultra-thin laptop with powerful performance', 1299.99, 1599.99, 18.75, 25, 5, electronics_id, 'CompuMax', true, false, 4.6, 234, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                (gen_random_uuid(), '4K Smart TV 55 inch', '4k-smart-tv-55', 'TV-55-4K-001', 'Immersive 4K Ultra HD Smart TV with HDR, built-in streaming apps, and voice control. Transform your living room.', 'Premium 4K Smart TV with HDR', 799.99, NULL, NULL, 15, 3, electronics_id, 'VisionTech', true, true, 4.8, 156, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                (gen_random_uuid(), 'Gaming Mouse RGB', 'gaming-mouse-rgb', 'GM-RGB-001', 'Precision gaming mouse with 16,000 DPI, customizable RGB lighting, and 8 programmable buttons. Dominate every game.', 'High-precision RGB gaming mouse', 79.99, 99.99, 20.00, 100, 20, electronics_id, 'GameGear', true, false, 4.3, 412, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

                -- Seed Sample Products - Clothing
                INSERT INTO products (id, name, slug, sku, description, short_description, price, original_price, discount_percentage, stock_quantity, low_stock_threshold, category_id, brand, is_active, is_featured, average_rating, total_reviews, created_at, updated_at)
                VALUES 
                (gen_random_uuid(), 'Mens Cotton T-Shirt', 'mens-cotton-tshirt', 'MCT-001', 'Comfortable 100% cotton t-shirt in multiple colors. Breathable fabric perfect for everyday wear.', 'Classic cotton t-shirt for men', 19.99, 29.99, 33.33, 200, 30, clothing_id, 'StyleHub', true, false, 4.2, 89, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                (gen_random_uuid(), 'Womens Denim Jeans', 'womens-denim-jeans', 'WDJ-001', 'Stylish skinny fit denim jeans with stretch fabric for maximum comfort. Available in multiple sizes.', 'Comfortable skinny fit jeans', 49.99, 69.99, 28.57, 150, 25, clothing_id, 'DenimCo', true, true, 4.6, 234, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                (gen_random_uuid(), 'Unisex Hoodie', 'unisex-hoodie', 'UH-001', 'Warm and cozy pullover hoodie with kangaroo pocket. Made from premium cotton blend.', 'Comfortable pullover hoodie', 39.99, NULL, NULL, 120, 20, clothing_id, 'ComfortWear', true, false, 4.4, 167, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

                -- Seed Sample Products - Books
                INSERT INTO products (id, name, slug, sku, description, short_description, price, original_price, discount_percentage, stock_quantity, low_stock_threshold, category_id, brand, is_active, is_featured, average_rating, total_reviews, created_at, updated_at)
                VALUES 
                (gen_random_uuid(), 'The Art of Programming', 'the-art-of-programming', 'TAP-001', 'Comprehensive guide to software development best practices and design patterns. A must-read for developers.', 'Essential programming guide', 59.99, 79.99, 25.00, 75, 10, books_id, 'TechPress', true, true, 4.9, 567, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                (gen_random_uuid(), 'Modern Web Development', 'modern-web-development', 'MWD-001', 'Learn React, Node.js, and modern web technologies from scratch. Includes practical projects.', 'Complete web development course', 44.99, 54.99, 18.18, 60, 10, books_id, 'CodeAcademy Press', true, false, 4.7, 289, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
            END $$;
        ");

        // Seed Product Images for featured products
        Execute.Sql(@"
            DO $$
            DECLARE
                product_rec RECORD;
            BEGIN
                FOR product_rec IN (SELECT id FROM products WHERE is_featured = true LIMIT 5)
                LOOP
                    INSERT INTO product_images (id, product_id, image_url, alt_text, is_primary, display_order, created_at)
                    VALUES 
                    (gen_random_uuid(), product_rec.id, 'https://via.placeholder.com/800x600?text=Product+Image+1', 'Primary product image', true, 1, CURRENT_TIMESTAMP),
                    (gen_random_uuid(), product_rec.id, 'https://via.placeholder.com/800x600?text=Product+Image+2', 'Secondary product image', false, 2, CURRENT_TIMESTAMP),
                    (gen_random_uuid(), product_rec.id, 'https://via.placeholder.com/800x600?text=Product+Image+3', 'Product detail image', false, 3, CURRENT_TIMESTAMP);
                END LOOP;
            END $$;
        ");

        // Seed Sample Reviews
        Execute.Sql(@"
            DO $$
            DECLARE
                customer_id UUID;
                product_rec RECORD;
            BEGIN
                SELECT id INTO customer_id FROM users WHERE email = 'customer@test.com' LIMIT 1;

                FOR product_rec IN (SELECT id FROM products WHERE is_featured = true LIMIT 3)
                LOOP
                    INSERT INTO reviews (id, product_id, user_id, rating, title, comment, is_verified_purchase, is_approved, helpful_count, created_at, updated_at)
                    VALUES 
                    (gen_random_uuid(), product_rec.id, customer_id, 5, 'Excellent product!', 'This product exceeded my expectations. Highly recommended!', true, true, 15, CURRENT_TIMESTAMP - INTERVAL '10 days', CURRENT_TIMESTAMP - INTERVAL '10 days'),
                    (gen_random_uuid(), product_rec.id, customer_id, 4, 'Good quality', 'Great product, but shipping took a bit longer than expected.', true, true, 8, CURRENT_TIMESTAMP - INTERVAL '5 days', CURRENT_TIMESTAMP - INTERVAL '5 days');
                END LOOP;
            END $$;
        ");

        // Seed Sample Coupons
        Execute.Sql(@"
            INSERT INTO coupons (id, code, description, discount_type, discount_value, min_order_amount, max_discount_amount, max_uses, max_uses_per_user, used_count, valid_from, valid_until, is_active, created_at, updated_at)
            VALUES 
            (gen_random_uuid(), 'WELCOME10', 'Welcome discount for new customers', 'Percentage', 10.00, 50.00, 20.00, 1000, 1, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '365 days', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'SAVE25', '25% off on orders above $100', 'Percentage', 25.00, 100.00, 50.00, 500, 3, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '90 days', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
            (gen_random_uuid(), 'FREESHIP', 'Free shipping on all orders', 'Fixed', 10.00, NULL, NULL, NULL, 1, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
        ");
    }

    public override void Down()
    {
        // Delete in reverse order to respect foreign keys
        Execute.Sql("DELETE FROM reviews;");
        Execute.Sql("DELETE FROM product_images;");
        Execute.Sql("DELETE FROM products;");
        Execute.Sql("DELETE FROM categories;");
        Execute.Sql("DELETE FROM coupons;");
        Execute.Sql("DELETE FROM users WHERE email IN ('admin@ecommerce.com', 'customer@test.com');");
    }
}

