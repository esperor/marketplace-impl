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
    IF (NEW."Status" = 4) THEN
		UPDATE inventory AS i
		SET "Quantity" = i."Quantity" + NEW."Quantity"
		WHERE "Id" = NEW."InventoryRecordId";
    END IF;

    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.restore_quantity_in_inventoryrecords()
    OWNER TO postgres;