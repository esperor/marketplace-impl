using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course.Server.Migrations
{
    /// <inheritdoc />
    public partial class update_functions_include_rating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS FN_GetProducts;

CREATE OR REPLACE FUNCTION FN_GetProducts(
    searchString varchar(100) DEFAULT NULL,
    storeId integer DEFAULT NULL,
    orderBy integer DEFAULT 0,
    ""offset"" integer DEFAULT 0,
    ""limit"" integer DEFAULT 10
)
RETURNS TABLE(
    ""Id"" int,
    ""StoreId"" int,
    ""Title"" text,
    ""Description"" text,
    ""RecordId"" int,
    ""Quantity"" int,
    ""Price"" int,
    ""Image"" bytea,
    ""PropertiesJson"" text,
    ""Size"" text,
    ""Variation"" text,
    ""StoreName"" text,
    ""Rating"" numeric(3, 2)
)
LANGUAGE sql
AS $$
SELECT
    p.""Id"",
    p.""StoreId"",
    p.""Title"",
    p.""Description"",
    i.""Id"" as ""RecordId"",
    i.""Quantity"",
    i.""Price"",
    i.""Image"",
    i.""PropertiesJson"",
    i.""Size"",
    i.""Variation"",
	s.""Name"" as ""StoreName"",
    Rating
FROM products p
LEFT JOIN inventory i ON i.""ProductId"" = p.""Id"" 
LEFT JOIN stores s ON p.""StoreId"" = s.""Id""
	CROSS JOIN LATERAL word_similarity(searchString, p.""Title"") similarityTitle
	CROSS JOIN LATERAL word_similarity(searchString, p.""Description"") similarityDescription
	CROSS JOIN LATERAL word_similarity(searchString, i.""Variation"") similarityVariation
	CROSS JOIN LATERAL word_similarity(searchString, s.""Name"") similarityName
	CROSS JOIN LATERAL (
		SELECT similarityName + similarityVariation + similarityDescription + similarityTitle AS similarity
	)
    CROSS JOIN LATERAL (
        SELECT AVG(rr.""RatingValue"") AS Rating
        FROM order_record orec
        LEFT JOIN rating_record rr ON rr.""OrderRecordId"" = orec.""Id""
        WHERE orec.""InventoryRecordId"" = i.""Id""
    )
WHERE
    (searchString IS NULL OR similarity > 0)
	AND (storeId IS NULL OR p.""StoreId"" = storeId)
ORDER BY
	similarity DESC,
    CASE WHEN orderBy = 0 THEN p.""Id"" END ASC,
    CASE WHEN orderBy = 1 THEN p.""Title"" END ASC,
    CASE WHEN orderBy = 2 THEN p.""Title"" END DESC,
    CASE WHEN orderBy = 3 THEN i.""Price"" END ASC,
    CASE WHEN orderBy = 4 THEN i.""Price"" END DESC
LIMIT ""limit""
OFFSET ""offset"";
$$;
");

            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS FN_GetOrders;

CREATE OR REPLACE FUNCTION public.FN_GetOrderRecords(
    sellerid integer DEFAULT NULL::integer,
    status integer DEFAULT NULL::integer,
	userid integer DEFAULT NULL::integer,
    storeid integer DEFAULT NULL::integer,
    orderid integer DEFAULT NULL::integer,
    ""offset"" integer DEFAULT 0,
    ""limit"" integer DEFAULT 10
)
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
    ""DelivererName"" text,
	""Rating"" numeric(3, 2)
)
LANGUAGE sql
AS $$
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
	du.""Name"" as ""DelivererName"",
	Rating
FROM orders o
INNER JOIN order_record orec ON o.""Id"" = orec.""OrderId""
INNER JOIN inventory i ON orec.""InventoryRecordId"" = i.""Id""
INNER JOIN products p ON p.""Id"" = i.""ProductId""
INNER JOIN stores s ON p.""StoreId"" = s.""Id""
LEFT JOIN users du ON o.""DelivererId"" = du.""Id""
	CROSS JOIN LATERAL (
		SELECT rr.""RatingValue"" AS Rating
		FROM rating_record rr
		WHERE rr.""OrderRecordId"" = orec.""Id""
	)
WHERE
	(sellerid IS NULL OR s.""OwnerId"" = sellerid)
	AND (storeId IS NULL OR p.""StoreId"" = storeId)
	AND (status IS NULL OR orec.""Status"" = status)
	AND (orderid IS NULL OR orec.""OrderId"" = orderid)
	AND (userid IS NULL OR o.""UserId"" = userid)
ORDER BY o.""Date"" DESC
LIMIT ""limit""
OFFSET ""offset"";
$$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS FN_GetProducts;

CREATE OR REPLACE FUNCTION FN_GetProducts(
    searchString varchar(100) DEFAULT NULL,
    storeId integer DEFAULT NULL,
    orderBy integer DEFAULT 0,
    ""offset"" integer DEFAULT 0,
    ""limit"" integer DEFAULT 10
)
RETURNS TABLE(
    ""Id"" int,
    ""StoreId"" int,
    ""Title"" text,
    ""Description"" text,
    ""RecordId"" int,
    ""Quantity"" int,
    ""Price"" int,
    ""Image"" bytea,
    ""PropertiesJson"" text,
    ""Size"" text,
    ""Variation"" text,
    ""StoreName"" text
)
LANGUAGE sql
AS $$
SELECT
    p.""Id"",
    p.""StoreId"",
    p.""Title"",
    p.""Description"",
    i.""Id"" as ""RecordId"",
    i.""Quantity"",
    i.""Price"",
    i.""Image"",
    i.""PropertiesJson"",
    i.""Size"",
    i.""Variation"",
	s.""Name"" as ""StoreName""
FROM products p
LEFT JOIN inventory i ON i.""ProductId"" = p.""Id"" 
LEFT JOIN stores s ON p.""StoreId"" = s.""Id""
	CROSS JOIN LATERAL word_similarity(searchString, p.""Title"") similarityTitle
	CROSS JOIN LATERAL word_similarity(searchString, p.""Description"") similarityDescription
	CROSS JOIN LATERAL word_similarity(searchString, i.""Variation"") similarityVariation
	CROSS JOIN LATERAL word_similarity(searchString, s.""Name"") similarityName
	CROSS JOIN LATERAL (
		SELECT similarityName + similarityVariation + similarityDescription + similarityTitle AS similarity
	)
WHERE
    (searchString IS NULL OR similarity > 0)
	AND (storeId IS NULL OR p.""StoreId"" = storeId)
ORDER BY
	similarity DESC,
    CASE WHEN orderBy = 0 THEN p.""Id"" END ASC,
    CASE WHEN orderBy = 1 THEN p.""Title"" END ASC,
    CASE WHEN orderBy = 2 THEN p.""Title"" END DESC,
    CASE WHEN orderBy = 3 THEN i.""Price"" END ASC,
    CASE WHEN orderBy = 4 THEN i.""Price"" END DESC
LIMIT ""limit""
OFFSET ""offset"";
$$;
");

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
    }
}
