using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace marketplace_impl.Server.Migrations
{
    /// <inheritdoc />
    public partial class move_orders_trigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP TRIGGER orders_restore_inventoryrecords_on_cancel
    ON public.orders;

CREATE OR REPLACE TRIGGER orderrecord_restore_inventoryrecords_on_cancel
    AFTER UPDATE 
    ON public.order_record
    FOR EACH ROW
    EXECUTE FUNCTION public.restore_quantity_in_inventoryrecords();

CREATE OR REPLACE FUNCTION public.restore_quantity_in_inventoryrecords()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

BEGIN
    IF (NEW.""Status"" = 4) THEN
		UPDATE inventory AS i
		SET ""Quantity"" = i.""Quantity"" + NEW.""Quantity""
		WHERE ""Id"" = NEW.""InventoryRecordId"";
    END IF;

    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.restore_quantity_in_inventoryrecords()
    OWNER TO postgres;

DROP TRIGGER orderrecord_subtract_from_inventoryrecords ON order_record;

CREATE OR REPLACE TRIGGER orderrecord_subtract_from_inventoryrecords_on_insert
    AFTER INSERT
    ON public.order_record
    FOR EACH ROW
    EXECUTE FUNCTION public.subtract_quantity_form_inventoryrecords();
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP TRIGGER orderrecord_restore_inventoryrecords_on_cancel
    ON public.order_record;

CREATE OR REPLACE TRIGGER orders_restore_inventoryrecords_on_cancel
    AFTER UPDATE 
    ON public.orders
    FOR EACH ROW
    EXECUTE FUNCTION public.restore_quantity_in_inventoryrecords();

CREATE OR REPLACE FUNCTION restore_quantity_in_inventoryRecords()
RETURNS TRIGGER
LANGUAGE PLPGSQL
AS
$$
DECLARE o_record RECORD;
BEGIN
    IF (NEW.""Status"" = 3) THEN
        FOR o_record IN
            SELECT ""InventoryRecordId"", ""Quantity"" 
            FROM order_record
            WHERE ""OrderId"" = NEW.""Id""
        LOOP
            UPDATE inventory AS i
            SET ""Quantity"" = i.""Quantity"" + o_record.""Quantity""
            WHERE ""Id"" = o_record.""InventoryRecordId"";
        END LOOP;
    END IF;

    RETURN NEW;
END;
$$;

DROP TRIGGER orderrecord_subtract_from_inventoryrecords_on_insert ON order_record;

CREATE OR REPLACE TRIGGER orderrecord_subtract_from_inventoryrecords
    AFTER INSERT
    ON public.order_record
    FOR EACH ROW
    EXECUTE FUNCTION public.subtract_quantity_form_inventoryrecords();
");
        }
    }
}
