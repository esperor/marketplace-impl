using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace marketplace_impl.Server.Migrations
{
    /// <inheritdoc />
    public partial class rename_vendors_stores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "vendors", newName: "stores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "stores", newName: "vendors");
        }
    }
}
