using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedPersonToTicketInfoTab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTabs",
                table: "UserTabs");

            migrationBuilder.RenameTable(
                name: "UserTabs",
                newName: "UserTab");

            migrationBuilder.AddColumn<int>(
                name: "AssignedPersonId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTab",
                table: "UserTab",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId",
                principalTable: "UserTab",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTab_AssignedPersonId",
                table: "TicketInfoTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoTabs_AssignedPersonId",
                table: "TicketInfoTabs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTab",
                table: "UserTab");

            migrationBuilder.DropColumn(
                name: "AssignedPersonId",
                table: "TicketInfoTabs");

            migrationBuilder.RenameTable(
                name: "UserTab",
                newName: "UserTabs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTabs",
                table: "UserTabs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
