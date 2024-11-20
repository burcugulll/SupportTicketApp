using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketInfoTabModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.AlterColumn<int>(
                name: "UserTabUserId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId",
                principalTable: "UserTab",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.AlterColumn<int>(
                name: "UserTabUserId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId",
                principalTable: "UserTab",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
