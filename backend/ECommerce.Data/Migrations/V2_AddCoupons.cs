using FluentMigrator;

namespace ECommerce.Data.Migrations;

[Migration(2, "Add coupons and coupon usage tracking tables")]
public class V2_AddCoupons : Migration
{
    public override void Up()
    {
        // Coupons table
        Create.Table("coupons")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("code").AsString(50).NotNullable().Unique()
            .WithColumn("description").AsString(500).Nullable()
            .WithColumn("discount_type").AsString(20).NotNullable() // 'Percentage' or 'Fixed'
            .WithColumn("discount_value").AsDecimal(10, 2).NotNullable()
            .WithColumn("min_order_amount").AsDecimal(10, 2).Nullable()
            .WithColumn("max_discount_amount").AsDecimal(10, 2).Nullable()
            .WithColumn("max_uses").AsInt32().Nullable()
            .WithColumn("max_uses_per_user").AsInt32().NotNullable().WithDefaultValue(1)
            .WithColumn("used_count").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("valid_from").AsDateTime().NotNullable()
            .WithColumn("valid_until").AsDateTime().NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("applies_to_categories").AsCustom("TEXT[]").Nullable()
            .WithColumn("applies_to_products").AsCustom("TEXT[]").Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.Index("ix_coupons_code").OnTable("coupons").OnColumn("code");
        Create.Index("ix_coupons_is_active").OnTable("coupons").OnColumn("is_active");
        Create.Index("ix_coupons_valid_dates").OnTable("coupons")
            .OnColumn("valid_from").Ascending()
            .OnColumn("valid_until").Ascending();

        // Coupon Usages table (track who used which coupons)
        Create.Table("coupon_usages")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("coupon_id").AsGuid().NotNullable()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("order_id").AsGuid().NotNullable()
            .WithColumn("discount_amount").AsDecimal(10, 2).NotNullable()
            .WithColumn("used_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_coupon_usages_coupons")
            .FromTable("coupon_usages").ForeignColumn("coupon_id")
            .ToTable("coupons").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_coupon_usages_users")
            .FromTable("coupon_usages").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_coupon_usages_orders")
            .FromTable("coupon_usages").ForeignColumn("order_id")
            .ToTable("orders").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_coupon_usages_coupon_id").OnTable("coupon_usages").OnColumn("coupon_id");
        Create.Index("ix_coupon_usages_user_id").OnTable("coupon_usages").OnColumn("user_id");
        Create.Index("ix_coupon_usages_order_id").OnTable("coupon_usages").OnColumn("order_id");
    }

    public override void Down()
    {
        Delete.Table("coupon_usages");
        Delete.Table("coupons");
    }
}
