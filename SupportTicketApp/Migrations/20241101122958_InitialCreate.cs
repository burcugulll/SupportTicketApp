using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketCommentImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCommentImages", x => x.ImageId);
                });

            migrationBuilder.CreateTable(
                name: "TicketImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketImages", x => x.ImageId);
                });

            migrationBuilder.CreateTable(
                name: "UserLogTabs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAdress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Log = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogTabs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "UserTabs",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProfilePhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
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
                    Urgency = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    TicketImageImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInfoTabs", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_TicketInfoTabs_TicketImages_TicketImageId",
                        column: x => x.TicketImageId,
                        principalTable: "TicketImages",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketInfoTabs_TicketImages_TicketImageImageId",
                        column: x => x.TicketImageImageId,
                        principalTable: "TicketImages",
                        principalColumn: "ImageId");
                });

            migrationBuilder.CreateTable(
                name: "TicketInfoCommentTabs",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TicketCommentImageId = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    TicketCommentImageImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInfoCommentTabs", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_TicketInfoCommentTabs_TicketCommentImages_TicketCommentImageId",
                        column: x => x.TicketCommentImageId,
                        principalTable: "TicketCommentImages",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketInfoCommentTabs_TicketCommentImages_TicketCommentImageImageId",
                        column: x => x.TicketCommentImageImageId,
                        principalTable: "TicketCommentImages",
                        principalColumn: "ImageId");
                    table.ForeignKey(
                        name: "FK_TicketInfoCommentTabs_TicketInfoTabs_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TicketInfoTabs",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_CommentImages_CommentId",
                table: "CommentImages",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentImages_ImageId",
                table: "CommentImages",
                column: "ImageId");

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
                name: "IX_TicketInfoCommentTabs_TicketId",
                table: "TicketInfoCommentTabs",
                column: "TicketId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentImages");

            migrationBuilder.DropTable(
                name: "UserLogTabs");

            migrationBuilder.DropTable(
                name: "UserTabs");

            migrationBuilder.DropTable(
                name: "TicketInfoCommentTabs");

            migrationBuilder.DropTable(
                name: "TicketCommentImages");

            migrationBuilder.DropTable(
                name: "TicketInfoTabs");

            migrationBuilder.DropTable(
                name: "TicketImages");
        }
    }
}
