using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class NewDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAssignments_TicketInfoTabs_TicketId",
                table: "TicketAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketAssignments_UserTabs_UserId",
                table: "TicketAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAssignments_TicketInfoTabs_TicketId",
                table: "TicketAssignments",
                column: "TicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAssignments_UserTabs_UserId",
                table: "TicketAssignments",
                column: "UserId",
                principalTable: "UserTabs",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAssignments_TicketInfoTabs_TicketId",
                table: "TicketAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketAssignments_UserTabs_UserId",
                table: "TicketAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAssignments_TicketInfoTabs_TicketId",
                table: "TicketAssignments",
                column: "TicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAssignments_UserTabs_UserId",
                table: "TicketAssignments",
                column: "UserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
