using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class MakeAddressNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Work = table.Column<string>(type: "text", nullable: true),
                    Hobby = table.Column<string>(type: "text", nullable: true),
                    FamilyInfo = table.Column<string>(type: "text", nullable: true),
                    ServicePreference = table.Column<string>(type: "text", nullable: true),
                    IsReferrer = table.Column<bool>(type: "boolean", nullable: false),
                    ReferredByClientId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Client_Client_ReferredByClientId",
                        column: x => x.ReferredByClientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ServiceType = table.Column<string>(type: "text", nullable: false),
                    PreferredDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsQuote = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequest_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServiceRequestId = table.Column<string>(type: "text", nullable: false),
                    ClientName = table.Column<string>(type: "text", nullable: false),
                    ProviderName = table.Column<string>(type: "text", nullable: false),
                    ServiceType = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_ServiceRequest_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderNotification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServiceRequestId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderNotification_ServiceRequest_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    JobId = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedback_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_ReferredByClientId",
                table: "Client",
                column: "ReferredByClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_JobId",
                table: "Feedback",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_ServiceRequestId",
                table: "Job",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderNotification_ServiceRequestId",
                table: "ProviderNotification",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequest_ClientId",
                table: "ServiceRequest",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "ProviderNotification");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "ServiceRequest");

            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}
