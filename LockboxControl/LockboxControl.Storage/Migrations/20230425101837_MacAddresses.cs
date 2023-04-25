using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockBoxControl.Storage.Migrations
{
    /// <inheritdoc />
    public partial class MacAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MacAddress",
                table: "Arduinos",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MacAddress",
                table: "Arduinos");
        }
    }
}
