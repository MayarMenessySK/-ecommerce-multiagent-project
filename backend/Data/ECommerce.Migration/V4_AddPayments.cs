using FluentMigrator;

namespace ECommerce.Migration;

[Migration(4, "Add payments and saved payment methods tables")]
public class V4_AddPayments : FluentMigrator.Migration
{
    public override void Up()
    {
        // Payments table (separate from orders for better tracking)
        Create.Table("payments")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("order_id").AsGuid().NotNullable()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("amount").AsDecimal(10, 2).NotNullable()
            .WithColumn("currency").AsString(3).NotNullable().WithDefaultValue("USD")
            .WithColumn("payment_method").AsString(50).NotNullable() // 'CreditCard', 'PayPal', 'Stripe', etc.
            .WithColumn("payment_provider").AsString(50).NotNullable() // 'Stripe', 'PayPal', etc.
            .WithColumn("transaction_id").AsString(255).Nullable()
            .WithColumn("stripe_payment_intent_id").AsString(255).Nullable()
            .WithColumn("paypal_order_id").AsString(255).Nullable()
            .WithColumn("status").AsString(50).NotNullable().WithDefaultValue("Pending") // 'Pending', 'Completed', 'Failed', 'Refunded'
            .WithColumn("failure_reason").AsString(500).Nullable()
            .WithColumn("refund_amount").AsDecimal(10, 2).Nullable()
            .WithColumn("refund_reason").AsString(500).Nullable()
            .WithColumn("refunded_at").AsDateTime().Nullable()
            .WithColumn("paid_at").AsDateTime().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_payments_orders")
            .FromTable("payments").ForeignColumn("order_id")
            .ToTable("orders").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.None);

        Create.ForeignKey("fk_payments_users")
            .FromTable("payments").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.None);

        Create.Index("ix_payments_order_id").OnTable("payments").OnColumn("order_id");
        Create.Index("ix_payments_user_id").OnTable("payments").OnColumn("user_id");
        Create.Index("ix_payments_status").OnTable("payments").OnColumn("status");
        Create.Index("ix_payments_transaction_id").OnTable("payments").OnColumn("transaction_id");
        Create.Index("ix_payments_stripe_intent").OnTable("payments").OnColumn("stripe_payment_intent_id");

        // Saved Payment Methods table (for recurring customers)
        Create.Table("saved_payment_methods")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("payment_provider").AsString(50).NotNullable()
            .WithColumn("payment_method_type").AsString(50).NotNullable() // 'card', 'bank_account'
            .WithColumn("stripe_payment_method_id").AsString(255).Nullable()
            .WithColumn("card_last_four").AsString(4).Nullable()
            .WithColumn("card_brand").AsString(50).Nullable() // 'visa', 'mastercard', 'amex'
            .WithColumn("card_exp_month").AsInt32().Nullable()
            .WithColumn("card_exp_year").AsInt32().Nullable()
            .WithColumn("is_default").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("billing_name").AsString(200).NotNullable()
            .WithColumn("billing_address_line1").AsString(255).Nullable()
            .WithColumn("billing_city").AsString(100).Nullable()
            .WithColumn("billing_state").AsString(100).Nullable()
            .WithColumn("billing_postal_code").AsString(20).Nullable()
            .WithColumn("billing_country").AsString(100).Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_saved_payment_methods_users")
            .FromTable("saved_payment_methods").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_saved_payment_methods_user_id").OnTable("saved_payment_methods").OnColumn("user_id");
        Create.Index("ix_saved_payment_methods_stripe").OnTable("saved_payment_methods").OnColumn("stripe_payment_method_id");
    }

    public override void Down()
    {
        Delete.Table("saved_payment_methods");
        Delete.Table("payments");
    }
}

