using FluentMigrator;

namespace ECommerce.Migration;

[Migration(3, "Add wishlists and wishlist items tables")]
public class V3_AddWishlists : FluentMigrator.Migration
{
    public override void Up()
    {
        // Wishlists table
        Create.Table("wishlists")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("name").AsString(100).NotNullable().WithDefaultValue("My Wishlist")
            .WithColumn("is_public").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("is_default").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_wishlists_users")
            .FromTable("wishlists").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_wishlists_user_id").OnTable("wishlists").OnColumn("user_id");

        // Wishlist Items table
        Create.Table("wishlist_items")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("wishlist_id").AsGuid().NotNullable()
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("added_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("notes").AsString(500).Nullable();

        Create.ForeignKey("fk_wishlist_items_wishlists")
            .FromTable("wishlist_items").ForeignColumn("wishlist_id")
            .ToTable("wishlists").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_wishlist_items_products")
            .FromTable("wishlist_items").ForeignColumn("product_id")
            .ToTable("products").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_wishlist_items_wishlist_id").OnTable("wishlist_items").OnColumn("wishlist_id");
        Create.Index("ix_wishlist_items_product_id").OnTable("wishlist_items").OnColumn("product_id");

        // Unique constraint: one product per wishlist
        Create.UniqueConstraint("uq_wishlist_items_wishlist_product")
            .OnTable("wishlist_items")
            .Columns("wishlist_id", "product_id");
    }

    public override void Down()
    {
        Delete.Table("wishlist_items");
        Delete.Table("wishlists");
    }
}

