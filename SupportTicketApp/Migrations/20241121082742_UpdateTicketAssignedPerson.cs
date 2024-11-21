using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketAssignedPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTab_AssignedPersonId",
                table: "TicketInfoTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoTabs_AssignedPersonId",
                table: "TicketInfoTabs");

            migrationBuilder.DropColumn(
                name: "AssignedPersonId",
                table: "TicketInfoTabs");

            migrationBuilder.AddColumn<int>(
                name: "TicketInfoTabTicketId",
                table: "UserTab",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTab_TicketInfoTabTicketId",
                table: "UserTab",
                column: "TicketInfoTabTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTab_TicketInfoTabs_TicketInfoTabTicketId",
                table: "UserTab",
                column: "TicketInfoTabTicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTab_TicketInfoTabs_TicketInfoTabTicketId",
                table: "UserTab");

            migrationBuilder.DropIndex(
                name: "IX_UserTab_TicketInfoTabTicketId",
                table: "UserTab");

            migrationBuilder.DropColumn(
                name: "TicketInfoTabTicketId",
                table: "UserTab");

            migrationBuilder.AddColumn<int>(
                name: "AssignedPersonId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoTabs_AssignedPersonId",
                table: "TicketInfoTabs",
                column: "AssignedPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTab_AssignedPersonId",
                table: "TicketInfoTabs",
                column: "AssignedPersonId",
                principalTable: "UserTab",
                principalColumn: "UserId");
        }
    }
}
