using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserTabs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserTabUserId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoTabs_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoTabs_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserTabs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropColumn(
                name: "UserTabUserId",
                table: "TicketInfoTabs");
        }
    }
}
