using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailReportingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HolidayCalendars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayDate = table.Column<DateOnly>(type: "date", nullable: false),
                    HolidayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HolidayNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HolidayType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurringMonth = table.Column<int>(type: "int", nullable: true),
                    RecurringDay = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HangfireJobId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TriggerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeriodStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenerationDurationMs = table.Column<long>(type: "bigint", nullable: true),
                    EmailDeliveryDurationMs = table.Column<long>(type: "bigint", nullable: true),
                    RecordsProcessed = table.Column<int>(type: "int", nullable: true),
                    RecipientsCount = table.Column<int>(type: "int", nullable: true),
                    SuccessfulDeliveries = table.Column<int>(type: "int", nullable: true),
                    FailedDeliveries = table.Column<int>(type: "int", nullable: true),
                    GeneratedFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExceptionStackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TriggeredByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TriggeredFromIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportExecutionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailDeliveryAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeliveryStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    FirstAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SmtpErrorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailSubject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AttachmentFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AttachmentSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailDeliveryAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailDeliveryAudits_ReportExecutionLogs_ReportExecutionId",
                        column: x => x.ReportExecutionId,
                        principalTable: "ReportExecutionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailDeliveryAudits_CreatedAt",
                table: "EmailDeliveryAudits",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDeliveryAudits_DeliveryStatus",
                table: "EmailDeliveryAudits",
                column: "DeliveryStatus");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDeliveryAudits_RecipientEmail",
                table: "EmailDeliveryAudits",
                column: "RecipientEmail");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDeliveryAudits_ReportExecutionId",
                table: "EmailDeliveryAudits",
                column: "ReportExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCalendars_HolidayDate",
                table: "HolidayCalendars",
                column: "HolidayDate");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCalendars_IsActive",
                table: "HolidayCalendars",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCalendars_Year_HolidayDate",
                table: "HolidayCalendars",
                columns: new[] { "Year", "HolidayDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportExecutionLogs_CreatedAt",
                table: "ReportExecutionLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReportExecutionLogs_ExecutionStatus",
                table: "ReportExecutionLogs",
                column: "ExecutionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ReportExecutionLogs_HangfireJobId",
                table: "ReportExecutionLogs",
                column: "HangfireJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportExecutionLogs_ReportType",
                table: "ReportExecutionLogs",
                column: "ReportType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailDeliveryAudits");

            migrationBuilder.DropTable(
                name: "HolidayCalendars");

            migrationBuilder.DropTable(
                name: "ReportExecutionLogs");
        }
    }
}
