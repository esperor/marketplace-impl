using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace course.Server.Migrations
{
    /// <inheritdoc />
    public partial class add_getOrdersBySeller_fn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_order_record",
                table: "order_record");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "order_record",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "order_record",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_record",
                table: "order_record",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_order_record_OrderId",
                table: "order_record",
                column: "OrderId");

            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION public.FN_GetOrders(
    sellerid integer DEFAULT NULL::integer,
    status integer DEFAULT NULL::integer,
    storeid integer DEFAULT NULL::integer,
    orderid integer DEFAULT NULL::integer,
    ""offset"" integer DEFAULT 0,
    ""limit"" integer DEFAULT 10)
 RETURNS TABLE(
    ""Id"" integer,
    ""OrderId"" integer,
    ""InventoryRecordId"" integer,
    ""ProductTitle"" text,
    ""ProductVariation"" text,
    ""Date"" date,
    ""Status"" integer,
    ""Quantity"" integer,
    ""Price"" integer,
    ""StoreId"" integer,
    ""StoreName"" text,
    ""DelivererContactInfo"" text,
    ""DelivererName"" text)
 LANGUAGE sql
AS $function$
SELECT
	orec.""Id"",
    orec.""OrderId"",
	orec.""InventoryRecordId"",
	p.""Title"",
	i.""Variation"",
	o.""Date"",
	orec.""Status"",
	orec.""Quantity"",
	i.""Price"",
	s.""Id"" as ""StoreId"",
	s.""Name"" as ""StoreName"",
	du.""Phone"" as ""DelivererContactInfo"",
	du.""Name"" as ""DelivererName""
FROM orders o
INNER JOIN order_record orec ON o.""Id"" = orec.""OrderId""
INNER JOIN inventory i ON orec.""InventoryRecordId"" = i.""Id""
INNER JOIN products p ON p.""Id"" = i.""ProductId""
INNER JOIN stores s ON p.""StoreId"" = s.""Id""
LEFT JOIN users du ON o.""DelivererId"" = du.""Id""
WHERE
	(sellerid IS NULL OR s.""OwnerId"" = sellerid)
	AND (storeId IS NULL OR p.""StoreId"" = storeId)
	AND (status IS NULL OR orec.""Status"" = status)
	AND (orderid IS NULL OR orec.""OrderId"" = orderid)
ORDER BY o.""Date"" DESC
LIMIT ""limit""
OFFSET ""offset"";
$function$
;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_order_record",
                table: "order_record");

            migrationBuilder.DropIndex(
                name: "IX_order_record_OrderId",
                table: "order_record");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "order_record");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "order_record");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_record",
                table: "order_record",
                columns: new[] { "OrderId", "InventoryRecordId" });

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS FN_GetOrdersBySeller;");
        }
    }
}
