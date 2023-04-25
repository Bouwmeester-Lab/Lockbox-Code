using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockBoxControl.Storage.Migrations
{
    /// <inheritdoc />
    public partial class ArduinoStatusLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ArduinoId",
                table: "ArduinoStatuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ArduinoStatuses_ArduinoId",
                table: "ArduinoStatuses",
                column: "ArduinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArduinoStatuses_Arduinos_ArduinoId",
                table: "ArduinoStatuses",
                column: "ArduinoId",
                principalTable: "Arduinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArduinoStatuses_Arduinos_ArduinoId",
                table: "ArduinoStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ArduinoStatuses_ArduinoId",
                table: "ArduinoStatuses");

            migrationBuilder.DropColumn(
                name: "ArduinoId",
                table: "ArduinoStatuses");
        }
    }
}
