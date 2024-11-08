using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginAttemptsAndLockoutEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEndTime",
                table: "UserTabs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginAttempts",
                table: "UserTabs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockoutEndTime",
                table: "UserTabs");

            migrationBuilder.DropColumn(
                name: "LoginAttempts",
                table: "UserTabs");
        }
    }
}
