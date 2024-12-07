using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketCommentImages_TicketCommentImageId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketCommentImages_TicketCommentImageImageId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_TicketImages_TicketImageId",
                table: "TicketInfoTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_TicketImages_TicketImageImageId",
                table: "TicketInfoTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTab_TicketInfoTabs_TicketInfoTabTicketId",
                table: "UserTab");

            migrationBuilder.DropTable(
                name: "CommentImages");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoTabs_TicketImageId",
                table: "TicketInfoTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoTabs_TicketImageImageId",
                table: "TicketInfoTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoTabs_UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoCommentTabs_TicketCommentImageId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoCommentTabs_TicketCommentImageImageId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketImages",
                table: "TicketImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketCommentImages",
                table: "TicketCommentImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTab",
                table: "UserTab");

            migrationBuilder.DropIndex(
                name: "IX_UserTab_TicketInfoTabTicketId",
                table: "UserTab");

            migrationBuilder.DropColumn(
                name: "TicketImageImageId",
                table: "TicketInfoTabs");

            migrationBuilder.DropColumn(
                name: "UserTabUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropColumn(
                name: "TicketCommentImageId",
                table: "TicketInfoCommentTabs");

            migrationBuilder.DropColumn(
                name: "TicketCommentImageImageId",
                table: "TicketInfoCommentTabs");

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

            migrationBuilder.DropColumn(
                name: "TicketInfoTabTicketId",
                table: "UserTab");

            migrationBuilder.RenameTable(
                name: "UserTab",
                newName: "UserTabs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TicketInfoTabs",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "TicketImages",
                newName: "TicketId");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "TicketCommentImages",
                newName: "ImagePath");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "TicketCommentImages",
                newName: "CommentId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserLogTabs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "TicketImageId",
                table: "TicketImages",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "TicketCommentImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TicketCommentImages",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "TicketInfoTabTicketId",
                table: "TicketCommentImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketImages",
                table: "TicketImages",
                column: "TicketImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketCommentImages",
                table: "TicketCommentImages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTabs",
                table: "UserTabs",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "TicketAssignments",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAssignments", x => new { x.TicketId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TicketAssignments_TicketInfoTabs_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TicketInfoTabs",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketAssignments_UserTabs_UserId",
                        column: x => x.UserId,
                        principalTable: "UserTabs",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLogTabs_UserId",
                table: "UserLogTabs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoTabs_CreatedByUserId",
                table: "TicketInfoTabs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketImages_TicketId",
                table: "TicketImages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentImages_CommentId",
                table: "TicketCommentImages",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentImages_TicketInfoTabTicketId",
                table: "TicketCommentImages",
                column: "TicketInfoTabTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAssignments_UserId",
                table: "TicketAssignments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                table: "TicketCommentImages",
                column: "CommentId",
                principalTable: "TicketInfoCommentTabs",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentImages_TicketInfoTabs_TicketInfoTabTicketId",
                table: "TicketCommentImages",
                column: "TicketInfoTabTicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketImages_TicketInfoTabs_TicketId",
                table: "TicketImages",
                column: "TicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_CreatedByUserId",
                table: "TicketInfoTabs",
                column: "CreatedByUserId",
                principalTable: "UserTabs",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogTabs_UserTabs_UserId",
                table: "UserLogTabs",
                column: "UserId",
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
                name: "FK_TicketCommentImages_TicketInfoTabs_TicketInfoTabTicketId",
                table: "TicketCommentImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketImages_TicketInfoTabs_TicketId",
                table: "TicketImages");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInfoTabs_UserTabs_CreatedByUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogTabs_UserTabs_UserId",
                table: "UserLogTabs");

            migrationBuilder.DropTable(
                name: "TicketAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserLogTabs_UserId",
                table: "UserLogTabs");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfoTabs_CreatedByUserId",
                table: "TicketInfoTabs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketImages",
                table: "TicketImages");

            migrationBuilder.DropIndex(
                name: "IX_TicketImages_TicketId",
                table: "TicketImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketCommentImages",
                table: "TicketCommentImages");

            migrationBuilder.DropIndex(
                name: "IX_TicketCommentImages_CommentId",
                table: "TicketCommentImages");

            migrationBuilder.DropIndex(
                name: "IX_TicketCommentImages_TicketInfoTabTicketId",
                table: "TicketCommentImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTabs",
                table: "UserTabs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserLogTabs");

            migrationBuilder.DropColumn(
                name: "TicketImageId",
                table: "TicketImages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TicketCommentImages");

            migrationBuilder.DropColumn(
                name: "TicketInfoTabTicketId",
                table: "TicketCommentImages");

            migrationBuilder.RenameTable(
                name: "UserTabs",
                newName: "UserTab");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "TicketInfoTabs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "TicketImages",
                newName: "ImageId");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "TicketCommentImages",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "TicketCommentImages",
                newName: "ImageId");

            migrationBuilder.AddColumn<int>(
                name: "TicketImageImageId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserTabUserId",
                table: "TicketInfoTabs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketCommentImageId",
                table: "TicketInfoCommentTabs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketCommentImageImageId",
                table: "TicketInfoCommentTabs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "TicketImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

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

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "TicketCommentImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

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

            migrationBuilder.AddColumn<int>(
                name: "TicketInfoTabTicketId",
                table: "UserTab",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketImages",
                table: "TicketImages",
                column: "ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketCommentImages",
                table: "TicketCommentImages",
                column: "ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTab",
                table: "UserTab",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "CommentImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentImages_TicketCommentImages_ImageId",
                        column: x => x.ImageId,
                        principalTable: "TicketCommentImages",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentImages_TicketInfoCommentTabs_CommentId",
                        column: x => x.CommentId,
                        principalTable: "TicketInfoCommentTabs",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoTabs_TicketImageId",
                table: "TicketInfoTabs",
                column: "TicketImageId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoTabs_TicketImageImageId",
                table: "TicketInfoTabs",
                column: "TicketImageImageId",
                unique: true,
                filter: "[TicketImageImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoTabs_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoCommentTabs_TicketCommentImageId",
                table: "TicketInfoCommentTabs",
                column: "TicketCommentImageId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoCommentTabs_TicketCommentImageImageId",
                table: "TicketInfoCommentTabs",
                column: "TicketCommentImageImageId",
                unique: true,
                filter: "[TicketCommentImageImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTab_TicketInfoTabTicketId",
                table: "UserTab",
                column: "TicketInfoTabTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentImages_CommentId",
                table: "CommentImages",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentImages_ImageId",
                table: "CommentImages",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketCommentImages_TicketCommentImageId",
                table: "TicketInfoCommentTabs",
                column: "TicketCommentImageId",
                principalTable: "TicketCommentImages",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoCommentTabs_TicketCommentImages_TicketCommentImageImageId",
                table: "TicketInfoCommentTabs",
                column: "TicketCommentImageImageId",
                principalTable: "TicketCommentImages",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_TicketImages_TicketImageId",
                table: "TicketInfoTabs",
                column: "TicketImageId",
                principalTable: "TicketImages",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_TicketImages_TicketImageImageId",
                table: "TicketInfoTabs",
                column: "TicketImageImageId",
                principalTable: "TicketImages",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInfoTabs_UserTab_UserTabUserId",
                table: "TicketInfoTabs",
                column: "UserTabUserId",
                principalTable: "UserTab",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTab_TicketInfoTabs_TicketInfoTabTicketId",
                table: "UserTab",
                column: "TicketInfoTabTicketId",
                principalTable: "TicketInfoTabs",
                principalColumn: "TicketId");
        }
    }
}
