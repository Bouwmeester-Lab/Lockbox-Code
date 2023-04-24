using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockboxControl.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Requests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArduinoId = table.Column<long>(type: "bigint", nullable: false),
                    CommandId = table.Column<long>(type: "bigint", nullable: false),
                    RequestDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Arduinos_ArduinoId",
                        column: x => x.ArduinoId,
                        principalTable: "Arduinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requests_Commands_CommandId",
                        column: x => x.CommandId,
                        principalTable: "Commands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ArduinoId",
                table: "Requests",
                column: "ArduinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CommandId",
                table: "Requests",
                column: "CommandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
