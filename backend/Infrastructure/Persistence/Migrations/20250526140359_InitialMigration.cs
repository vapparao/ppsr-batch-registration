using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchOperations",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileChecksum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    ValidRecords = table.Column<int>(type: "int", nullable: false),
                    InvalidRecords = table.Column<int>(type: "int", nullable: false),
                    AddedRecords = table.Column<int>(type: "int", nullable: false),
                    UpdatedRecords = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchOperations", x => x.BatchId);
                    table.CheckConstraint("CK_batch_operation_status", "status IN ('PROCESSING', 'COMPLETED', 'FAILED')");
                });

            migrationBuilder.CreateTable(
                name: "Grantors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleNames = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grantors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spgs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Acn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spgs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvalidRecords",
                columns: table => new
                {
                    InvalidRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    RecordData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvalidRecords", x => x.InvalidRecordId);
                    table.ForeignKey(
                        name: "FK_InvalidRecords_BatchOperations_BatchId",
                        column: x => x.BatchId,
                        principalTable: "BatchOperations",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    GrantorId = table.Column<int>(type: "int", nullable: false),
                    SpgId = table.Column<int>(type: "int", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_BatchOperations_BatchId",
                        column: x => x.BatchId,
                        principalTable: "BatchOperations",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_Grantors_GrantorId",
                        column: x => x.GrantorId,
                        principalTable: "Grantors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_Spgs_SpgId",
                        column: x => x.SpgId,
                        principalTable: "Spgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvalidRecords_BatchId",
                table: "InvalidRecords",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_BatchId",
                table: "Registrations",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_GrantorId",
                table: "Registrations",
                column: "GrantorId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_SpgId",
                table: "Registrations",
                column: "SpgId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvalidRecords");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "BatchOperations");

            migrationBuilder.DropTable(
                name: "Grantors");

            migrationBuilder.DropTable(
                name: "Spgs");
        }
    }
}
