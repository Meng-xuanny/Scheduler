using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "ServiceRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "ProviderNotification",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    Skills = table.Column<string>(type: "text", nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequest_ProviderId",
                table: "ServiceRequest",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderNotification_ProviderId",
                table: "ProviderNotification",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderNotification_Provider_ProviderId",
                table: "ProviderNotification",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequest_Provider_ProviderId",
                table: "ServiceRequest",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderNotification_Provider_ProviderId",
                table: "ProviderNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequest_Provider_ProviderId",
                table: "ServiceRequest");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequest_ProviderId",
                table: "ServiceRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProviderNotification_ProviderId",
                table: "ProviderNotification");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "ServiceRequest");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "ProviderNotification");
        }
    }
}
