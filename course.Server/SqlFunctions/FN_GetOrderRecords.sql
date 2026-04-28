CREATE OR REPLACE FUNCTION public.FN_GetOrderRecords(
    sellerid integer DEFAULT NULL::integer,
    status integer DEFAULT NULL::integer,
	userid integer DEFAULT NULL::integer,
    storeid integer DEFAULT NULL::integer,
    orderid integer DEFAULT NULL::integer,
    "offset" integer DEFAULT 0,
    "limit" integer DEFAULT 10
)
RETURNS TABLE(
    "Id" integer,
    "OrderId" integer,
    "InventoryRecordId" integer,
    "ProductTitle" text,
    "ProductVariation" text,
    "Date" date,
    "Status" integer,
    "Quantity" integer,
    "Price" integer,
    "StoreId" integer,
    "StoreName" text,
    "DelivererContactInfo" text,
    "DelivererName" text,
	"RatingValue" integer,
	"RatingComment" text,
	"RatingDate" date,
	"UserId" integer,
	"Address" text
)
LANGUAGE sql
AS $$
SELECT
	orec."Id",
    orec."OrderId",
	orec."InventoryRecordId",
	p."Title" as "ProductTitle",
	i."Variation" as "ProductVariation",
	o."Date",
	orec."Status",
	orec."Quantity",
	i."Price",
	s."Id" as "StoreId",
	s."Name" as "StoreName",
	du."Phone" as "DelivererContactInfo",
	du."Name" as "DelivererName",
	orec."RatingValue",
	orec."RatingComment",
	orec."RatingDate",
	o."UserId",
	o."Address"
FROM orders o
INNER JOIN order_record orec ON o."Id" = orec."OrderId"
INNER JOIN inventory i ON orec."InventoryRecordId" = i."Id"
INNER JOIN products p ON p."Id" = i."ProductId"
INNER JOIN stores s ON p."StoreId" = s."Id"
LEFT JOIN users du ON o."DelivererId" = du."Id"
WHERE
	(sellerid IS NULL OR s."OwnerId" = sellerid)
	AND (storeId IS NULL OR p."StoreId" = storeId)
	AND (status IS NULL OR orec."Status" = status)
	AND (orderid IS NULL OR orec."OrderId" = orderid)
	AND (userid IS NULL OR o."UserId" = userid)
ORDER BY o."Date" DESC
LIMIT "limit"
OFFSET "offset";
$$;