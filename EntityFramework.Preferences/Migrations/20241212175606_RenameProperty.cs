using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Preferences.Migrations
{
    /// <inheritdoc />
    public partial class RenameProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureTime",
                table: "Ticket",
                newName: "DepartureDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureDate",
                table: "Ticket",
                newName: "DepartureTime");
        }
    }
}
