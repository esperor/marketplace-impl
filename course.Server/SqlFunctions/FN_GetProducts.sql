CREATE OR REPLACE FUNCTION FN_GetProducts(
    searchString varchar(100) DEFAULT NULL,
    storeId integer DEFAULT NULL,
    orderBy integer DEFAULT 0,
    productid integer DEFAULT NULL,
    "offset" integer DEFAULT 0,
    "limit" integer DEFAULT 10
)
RETURNS TABLE(
    "ProductId" int,
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
    p."Id" as "ProductId",
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
	CROSS JOIN LATERAL (
        SELECT CASE WHEN searchString is not null THEN word_similarity(searchString, p."Title") ELSE NULL END AS similarityTitle
    )
	CROSS JOIN LATERAL (
        SELECT CASE WHEN searchString is not null THEN word_similarity(searchString, p."Description") ELSE NULL END AS similarityDescription
    )
	CROSS JOIN LATERAL (
        SELECT CASE WHEN searchString is not null THEN word_similarity(searchString, i."Variation") ELSE NULL END AS similarityVariation
    )
	CROSS JOIN LATERAL (
        SELECT CASE WHEN searchString is not null THEN word_similarity(searchString, s."Name") ELSE NULL END AS similarityName
    )
	CROSS JOIN LATERAL (
		SELECT CASE WHEN searchString is not null 
            THEN similarityName + similarityVariation + similarityDescription + similarityTitle
            ELSE NULL
        END AS similarity
	)
    CROSS JOIN LATERAL (
        SELECT AVG(orec."RatingValue") AS Rating
        FROM order_record orec
        WHERE orec."InventoryRecordId" = i."Id"
    )
WHERE
    (searchString IS NULL OR similarity > 0)
	AND (storeId IS NULL OR p."StoreId" = storeId)
    AND (productid IS NULL OR p."Id" = productid)
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