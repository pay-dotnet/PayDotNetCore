using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialPayMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Processor = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    ProcessorId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayMerchant",
                columns: table => new
                {
                    ProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOnboardingComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    Processor = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayMerchant", x => x.ProcessorId);
                });

            migrationBuilder.CreateTable(
                name: "PayWebhook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Event = table.Column<string>(type: "TEXT", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", nullable: false),
                    Processor = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayWebhook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayPaymentMethod",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayPaymentMethod", x => new { x.CustomerId, x.ProcessorId });
                    table.ForeignKey(
                        name: "FK_PayPaymentMethod_PayCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "PayCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaySubscription",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    ApplicationFeePercent = table.Column<decimal>(type: "TEXT", precision: 8, scale: 2, nullable: true),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CurrentPeriodStart = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndsAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsMetered = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PauseBehaviour = table.Column<int>(type: "INTEGER", nullable: true),
                    PauseResumesAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PauseStartsAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProcessorPlan = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    TrailEndsAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaySubscription", x => new { x.CustomerId, x.ProcessorId });
                    table.ForeignKey(
                        name: "FK_PaySubscription_PayCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "PayCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayCharge",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    AmountRefunded = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationFeeAmount = table.Column<int>(type: "INTEGER", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    SubscriptionId = table.Column<string>(type: "TEXT", nullable: true),
                    AmountCaptured = table.Column<int>(type: "INTEGER", nullable: false),
                    Bank = table.Column<string>(type: "TEXT", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    Discounts = table.Column<string>(type: "TEXT", nullable: false),
                    ExpirationMonth = table.Column<string>(type: "TEXT", nullable: true),
                    ExpirationYear = table.Column<string>(type: "TEXT", nullable: true),
                    InvoiceId = table.Column<string>(type: "TEXT", nullable: true),
                    Last4 = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentIntentId = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentMethodType = table.Column<string>(type: "TEXT", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceiptUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Subtotal = table.Column<int>(type: "INTEGER", nullable: false),
                    Tax = table.Column<int>(type: "INTEGER", nullable: true),
                    PaySubscriptionCustomerId = table.Column<string>(type: "TEXT", nullable: true),
                    PaySubscriptionProcessorId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCharge", x => new { x.CustomerId, x.ProcessorId });
                    table.ForeignKey(
                        name: "FK_PayCharge_PayCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "PayCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayCharge_PaySubscription_PaySubscriptionCustomerId_PaySubscriptionProcessorId",
                        columns: x => new { x.PaySubscriptionCustomerId, x.PaySubscriptionProcessorId },
                        principalTable: "PaySubscription",
                        principalColumns: new[] { "CustomerId", "ProcessorId" });
                });

            migrationBuilder.CreateTable(
                name: "PaySubscriptionItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PaySubscriptionCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    PaySubscriptionProcessorId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaySubscriptionItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaySubscriptionItem_PaySubscription_PaySubscriptionCustomerId_PaySubscriptionProcessorId",
                        columns: x => new { x.PaySubscriptionCustomerId, x.PaySubscriptionProcessorId },
                        principalTable: "PaySubscription",
                        principalColumns: new[] { "CustomerId", "ProcessorId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayCharge_TotalTaxAmounts",
                columns: table => new
                {
                    PayChargeCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    PayChargeProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCharge_TotalTaxAmounts", x => new { x.PayChargeCustomerId, x.PayChargeProcessorId, x.Id });
                    table.ForeignKey(
                        name: "FK_PayCharge_TotalTaxAmounts_PayCharge_PayChargeCustomerId_PayChargeProcessorId",
                        columns: x => new { x.PayChargeCustomerId, x.PayChargeProcessorId },
                        principalTable: "PayCharge",
                        principalColumns: new[] { "CustomerId", "ProcessorId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayChargeLineItem",
                columns: table => new
                {
                    PayChargeCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    PayChargeProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Discounts = table.Column<string>(type: "TEXT", nullable: false),
                    IsProration = table.Column<bool>(type: "INTEGER", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PriceId = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitAmount = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayChargeLineItem", x => new { x.PayChargeCustomerId, x.PayChargeProcessorId, x.Id });
                    table.ForeignKey(
                        name: "FK_PayChargeLineItem_PayCharge_PayChargeCustomerId_PayChargeProcessorId",
                        columns: x => new { x.PayChargeCustomerId, x.PayChargeProcessorId },
                        principalTable: "PayCharge",
                        principalColumns: new[] { "CustomerId", "ProcessorId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayChargeRefund",
                columns: table => new
                {
                    PayChargeCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    PayChargeProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayChargeRefund", x => new { x.PayChargeCustomerId, x.PayChargeProcessorId, x.Id });
                    table.ForeignKey(
                        name: "FK_PayChargeRefund_PayCharge_PayChargeCustomerId_PayChargeProcessorId",
                        columns: x => new { x.PayChargeCustomerId, x.PayChargeProcessorId },
                        principalTable: "PayCharge",
                        principalColumns: new[] { "CustomerId", "ProcessorId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayChargeTotalDiscount",
                columns: table => new
                {
                    PayChargeCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    PayChargeProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DiscountId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayChargeTotalDiscount", x => new { x.PayChargeCustomerId, x.PayChargeProcessorId, x.Id });
                    table.ForeignKey(
                        name: "FK_PayChargeTotalDiscount_PayCharge_PayChargeCustomerId_PayChargeProcessorId",
                        columns: x => new { x.PayChargeCustomerId, x.PayChargeProcessorId },
                        principalTable: "PayCharge",
                        principalColumns: new[] { "CustomerId", "ProcessorId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayChargeLineItem_TaxAmounts",
                columns: table => new
                {
                    PayChargeLineItemPayChargeCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    PayChargeLineItemPayChargeProcessorId = table.Column<string>(type: "TEXT", nullable: false),
                    PayChargeLineItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayChargeLineItem_TaxAmounts", x => new { x.PayChargeLineItemPayChargeCustomerId, x.PayChargeLineItemPayChargeProcessorId, x.PayChargeLineItemId, x.Id });
                    table.ForeignKey(
                        name: "FK_PayChargeLineItem_TaxAmounts_PayChargeLineItem_PayChargeLineItemPayChargeCustomerId_PayChargeLineItemPayChargeProcessorId_PayChargeLineItemId",
                        columns: x => new { x.PayChargeLineItemPayChargeCustomerId, x.PayChargeLineItemPayChargeProcessorId, x.PayChargeLineItemId },
                        principalTable: "PayChargeLineItem",
                        principalColumns: new[] { "PayChargeCustomerId", "PayChargeProcessorId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayCharge_PaySubscriptionCustomerId_PaySubscriptionProcessorId",
                table: "PayCharge",
                columns: new[] { "PaySubscriptionCustomerId", "PaySubscriptionProcessorId" });

            migrationBuilder.CreateIndex(
                name: "IX_PayCustomer_Processor_ProcessorId",
                table: "PayCustomer",
                columns: new[] { "Processor", "ProcessorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaySubscriptionItem_PaySubscriptionCustomerId_PaySubscriptionProcessorId",
                table: "PaySubscriptionItem",
                columns: new[] { "PaySubscriptionCustomerId", "PaySubscriptionProcessorId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayCharge_TotalTaxAmounts");

            migrationBuilder.DropTable(
                name: "PayChargeLineItem_TaxAmounts");

            migrationBuilder.DropTable(
                name: "PayChargeRefund");

            migrationBuilder.DropTable(
                name: "PayChargeTotalDiscount");

            migrationBuilder.DropTable(
                name: "PayMerchant");

            migrationBuilder.DropTable(
                name: "PayPaymentMethod");

            migrationBuilder.DropTable(
                name: "PaySubscriptionItem");

            migrationBuilder.DropTable(
                name: "PayWebhook");

            migrationBuilder.DropTable(
                name: "PayChargeLineItem");

            migrationBuilder.DropTable(
                name: "PayCharge");

            migrationBuilder.DropTable(
                name: "PaySubscription");

            migrationBuilder.DropTable(
                name: "PayCustomer");
        }
    }
}
