CREATE OR REPLACE FUNCTION FN_GetProducts(
    searchString varchar(100) DEFAULT NULL,
    storeId integer DEFAULT NULL,
    orderBy integer DEFAULT 0,
    "offset" integer DEFAULT 0,
    "limit" integer DEFAULT 10
)
RETURNS TABLE(
    "Id" int,
    "StoreId" int,
    "Title" text,
    "Description" text,
    "RecordId" int,
    "Quantity" int,
    "Price" int,
    "Image" bytea,
    "PropertiesJson" text,
    "Size" text,
    "Variation" text,
    "StoreName" text,
    "Rating" numeric(3, 2)
)
LANGUAGE sql
AS $$
SELECT
    p."Id",
    p."StoreId",
    p."Title",
    p."Description",
    i."Id" as "RecordId",
    i."Quantity",
    i."Price",
    i."Image",
    i."PropertiesJson",
    i."Size",
    i."Variation",
	s."Name" as "StoreName",
    Rating
FROM products p
LEFT JOIN inventory i ON i."ProductId" = p."Id" 
LEFT JOIN stores s ON p."StoreId" = s."Id"
	CROSS JOIN LATERAL word_similarity(searchString, p."Title") similarityTitle
	CROSS JOIN LATERAL word_similarity(searchString, p."Description") similarityDescription
	CROSS JOIN LATERAL word_similarity(searchString, i."Variation") similarityVariation
	CROSS JOIN LATERAL word_similarity(searchString, s."Name") similarityName
	CROSS JOIN LATERAL (
		SELECT similarityName + similarityVariation + similarityDescription + similarityTitle AS similarity
	)
    CROSS JOIN LATERAL (
        SELECT AVG(rr."RatingValue") AS Rating
        FROM order_record orec
        LEFT JOIN rating_record rr ON rr."OrderRecordId" = orec."Id"
        WHERE orec."InventoryRecordId" = i."Id"
    )
WHERE
    (searchString IS NULL OR similarity > 0)
	AND (storeId IS NULL OR p."StoreId" = storeId)
ORDER BY
	similarity DESC,
    CASE WHEN orderBy = 0 THEN p."Id" END ASC,
    CASE WHEN orderBy = 1 THEN p."Title" END ASC,
    CASE WHEN orderBy = 2 THEN p."Title" END DESC,
    CASE WHEN orderBy = 3 THEN i."Price" END ASC,
    CASE WHEN orderBy = 4 THEN i."Price" END DESC
LIMIT "limit"
OFFSET "offset";
$$;