using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobTableWithClientConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClientConfirmedAt",
                table: "Job",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClientConfirmed",
                table: "Job",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientConfirmedAt",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "IsClientConfirmed",
                table: "Job");
        }
    }
}
