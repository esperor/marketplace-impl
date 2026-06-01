CREATE OR REPLACE TRIGGER orderrecord_subtract_from_inventoryrecords_on_insert
    AFTER INSERT
    ON public.order_record
    FOR EACH ROW
    EXECUTE FUNCTION public.subtract_quantity_form_inventoryrecords();

CREATE OR REPLACE FUNCTION public.subtract_quantity_form_inventoryrecords()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

BEGIN
    UPDATE inventory AS i
    SET "Quantity" = i."Quantity" - NEW."Quantity"
    WHERE "Id" = NEW."InventoryRecordId";

    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.subtract_quantity_form_inventoryrecords()
    OWNER TO postgres;
