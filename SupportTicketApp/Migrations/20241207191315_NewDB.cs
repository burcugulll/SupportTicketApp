using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class NewDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTabs",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProfilePhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    LoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LockoutEndTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTabs", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TicketInfoTabs",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketImageId = table.Column<int>(type: "int", nullable: true),
                    Urgency = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInfoTabs", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_TicketInfoTabs_UserTabs_UserId",
                        column: x => x.UserId,
                        principalTable: "UserTabs",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogTabs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAdress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Log = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogTabs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_UserLogTabs_UserTabs_UserId",
                        column: x => x.UserId,
                        principalTable: "UserTabs",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        principalColumn: "TicketId");
                    table.ForeignKey(
                        name: "FK_TicketAssignments_UserTabs_UserId",
                        column: x => x.UserId,
                        principalTable: "UserTabs",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TicketImages",
                columns: table => new
                {
                    TicketImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketImages", x => x.TicketImageId);
                    table.ForeignKey(
                        name: "FK_TicketImages_TicketInfoTabs_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TicketInfoTabs",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketInfoCommentTabs",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInfoCommentTabs", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_TicketInfoCommentTabs_TicketInfoTabs_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TicketInfoTabs",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketCommentImages",
                columns: table => new
                {
                    CommentImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCommentImages", x => x.CommentImageId);
                    table.ForeignKey(
                        name: "FK_TicketCommentImages_TicketInfoCommentTabs_CommentId",
                        column: x => x.CommentId,
                        principalTable: "TicketInfoCommentTabs",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketAssignments_UserId",
                table: "TicketAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentImages_CommentId",
                table: "TicketCommentImages",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketImages_TicketId",
                table: "TicketImages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoCommentTabs_TicketId",
                table: "TicketInfoCommentTabs",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfoTabs_UserId",
                table: "TicketInfoTabs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogTabs_UserId",
                table: "UserLogTabs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketAssignments");

            migrationBuilder.DropTable(
                name: "TicketCommentImages");

            migrationBuilder.DropTable(
                name: "TicketImages");

            migrationBuilder.DropTable(
                name: "UserLogTabs");

            migrationBuilder.DropTable(
                name: "TicketInfoCommentTabs");

            migrationBuilder.DropTable(
                name: "TicketInfoTabs");

            migrationBuilder.DropTable(
                name: "UserTabs");
        }
    }
}
