using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class mig_update_ticketCommentTab : Migration
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

            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoCommentTabs_UserTabs_UserId",
                table: "TicketInfoCommentTabs");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages",
                column: "CommentId",
                principalTable: "TicketInfoCommentTabs",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoCommentTabs_UserTabs_UserId",
                table: "TicketInfoCommentTabs",
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

            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoCommentTabs_UserTabs_UserId",
                table: "TicketInfoCommentTabs");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages",
                column: "CommentId",
                principalTable: "TicketInfoCommentTabs",
                principalColumn: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoCommentTabs_UserTabs_UserId",
                table: "TicketInfoCommentTabs",
                column: "UserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
