using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course.Server.Migrations
{
    /// <inheritdoc />
    public partial class add_rating_columns_to_order_record : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RatingComment",
                table: "order_record",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "RatingDate",
                table: "order_record",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingValue",
                table: "order_record",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingComment",
                table: "order_record");

            migrationBuilder.DropColumn(
                name: "RatingDate",
                table: "order_record");

            migrationBuilder.DropColumn(
                name: "RatingValue",
                table: "order_record");
        }
    }
}
