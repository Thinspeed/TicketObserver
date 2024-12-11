using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Preferences.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Train",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Train");
        }
    }
}
