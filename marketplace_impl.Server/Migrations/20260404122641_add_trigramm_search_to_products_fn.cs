using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace marketplace_impl.Server.Migrations
{
    /// <inheritdoc />
    public partial class add_trigramm_search_to_products_fn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION pg_trgm;");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // old FN_GetProducts version
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
WHERE
    (searchString IS NULL OR p.""Title"" ILIKE '%' || searchString || '%')
AND (storeId IS NULL OR p.""StoreId"" = storeId)
ORDER BY
    CASE WHEN orderBy = 0 THEN p.""Id"" END ASC,
    CASE WHEN orderBy = 1 THEN p.""Title"" END ASC,
    CASE WHEN orderBy = 2 THEN p.""Title"" END DESC,
    CASE WHEN orderBy = 3 THEN i.""Price"" END ASC,
    CASE WHEN orderBy = 4 THEN i.""Price"" END DESC
LIMIT ""limit""
OFFSET ""offset"";
$$;
");

            migrationBuilder.Sql("DROP EXTENSION pg_trgm;");
        }
    }
}
