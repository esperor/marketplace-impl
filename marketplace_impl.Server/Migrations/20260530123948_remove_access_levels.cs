using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace marketplace_impl.Server.Migrations
{
    /// <inheritdoc />
    public partial class remove_access_levels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_access_levels_AccessLevelId",
                table: "users");

            migrationBuilder.DropTable(
                name: "access_levels");

            migrationBuilder.DropIndex(
                name: "IX_users_AccessLevelId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "AccessLevelId",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessLevelId",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "access_levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_levels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_AccessLevelId",
                table: "users",
                column: "AccessLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_access_levels_AccessLevelId",
                table: "users",
                column: "AccessLevelId",
                principalTable: "access_levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
