using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToProviderNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProviderNotification",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProviderNotification");
        }
    }
}
