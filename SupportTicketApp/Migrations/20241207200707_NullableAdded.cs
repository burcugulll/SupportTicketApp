using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class NullableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketInfoTabs_TicketId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TicketInfoCommentTabs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserTabUserId",
                table: "TicketInfoCommentTabs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoCommentTabs_UserTabUserId",
                table: "TicketInfoCommentTabs",
                column: "UserTabUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages",
                column: "CommentId",
                principalTable: "TicketInfoCommentTabs",
                principalColumn: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketInfoTabs_TicketId",
                table: "TicketInfoCommentTabs",
                column: "TicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoCommentTabs_UserTabs_UserTabUserId",
                table: "TicketInfoCommentTabs",
                column: "UserTabUserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketInfoTabs_TicketId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoCommentTabs_UserTabs_UserTabUserId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoCommentTabs_UserTabUserId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropColumn(
                name: "UserTabUserId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages",
                column: "CommentId",
                principalTable: "TicketInfoCommentTabs",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketInfoTabs_TicketId",
                table: "TicketInfoCommentTabs",
                column: "TicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
