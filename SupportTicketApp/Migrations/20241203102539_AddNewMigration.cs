using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentImages_TicketInfoTabs_TicketInfoTabTicketId",
                table: "TicketCommentImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_CreatedByUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketCommentImages_TicketInfoTabTicketId",
                table: "TicketCommentImages");

            migrationBuilder.DropColumn(
                name: "TicketInfoTabTicketId",
                table: "TicketCommentImages");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "TicketInfoTabs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketInfoTabs_CreatedByUserId",
                table: "TicketInfoTabs",
                newName: "IX_TicketInfoTabs_UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TicketCommentImages",
                newName: "CommentImageId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "TicketImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "TicketImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "TicketImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "TicketCommentImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "TicketCommentImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "TicketCommentImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_UserId",
                table: "TicketInfoTabs",
                column: "UserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_UserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TicketImages");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "TicketImages");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "TicketImages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TicketImages");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "TicketCommentImages");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "TicketCommentImages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TicketCommentImages");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TicketInfoTabs",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketInfoTabs_UserId",
                table: "TicketInfoTabs",
                newName: "IX_TicketInfoTabs_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CommentImageId",
                table: "TicketCommentImages",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "TicketInfoTabTicketId",
                table: "TicketCommentImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentImages_TicketInfoTabTicketId",
                table: "TicketCommentImages",
                column: "TicketInfoTabTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentImages_TicketInfoTabs_TicketInfoTabTicketId",
                table: "TicketCommentImages",
                column: "TicketInfoTabTicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_CreatedByUserId",
                table: "TicketInfoTabs",
                column: "CreatedByUserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
