using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sybon.Checking.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Output = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceLimits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemoryLimitBytes = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfProcesses = table.Column<long>(type: "bigint", nullable: false),
                    OutputLimitBytes = table.Column<long>(type: "bigint", nullable: false),
                    RealTimeLimitMillis = table.Column<long>(type: "bigint", nullable: false),
                    TimeLimitMillis = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceLimits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceUsage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemoryUsageBytes = table.Column<long>(type: "bigint", nullable: false),
                    TimeUsageMillis = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceUsage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solution",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubmitResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BuildResultId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmitResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitResult_BuildResult_BuildResultId",
                        column: x => x.BuildResultId,
                        principalTable: "BuildResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compilers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Args = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceLimitsId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compilers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compilers_ResourceLimits_ResourceLimitsId",
                        column: x => x.ResourceLimitsId,
                        principalTable: "ResourceLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestGroupResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Executed = table.Column<bool>(type: "bit", nullable: false),
                    InternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitResultId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroupResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestGroupResult_SubmitResult_SubmitResultId",
                        column: x => x.SubmitResultId,
                        principalTable: "SubmitResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompilerId = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PretestsOnly = table.Column<bool>(type: "bit", nullable: false),
                    ProblemId = table.Column<long>(type: "bigint", nullable: false),
                    ResultId = table.Column<long>(type: "bigint", nullable: true),
                    SolutionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submits_Compilers_CompilerId",
                        column: x => x.CompilerId,
                        principalTable: "Compilers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submits_SubmitResult_ResultId",
                        column: x => x.ResultId,
                        principalTable: "SubmitResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submits_Solution_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActualResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JudgeMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceUsageId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TestGroupResultId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResult_ResourceUsage_ResourceUsageId",
                        column: x => x.ResourceUsageId,
                        principalTable: "ResourceUsage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResult_TestGroupResult_TestGroupResultId",
                        column: x => x.TestGroupResultId,
                        principalTable: "TestGroupResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compilers_ResourceLimitsId",
                table: "Compilers",
                column: "ResourceLimitsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitResult_BuildResultId",
                table: "SubmitResult",
                column: "BuildResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_CompilerId",
                table: "Submits",
                column: "CompilerId");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_ResultId",
                table: "Submits",
                column: "ResultId",
                unique: true,
                filter: "[ResultId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_SolutionId",
                table: "Submits",
                column: "SolutionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupResult_SubmitResultId",
                table: "TestGroupResult",
                column: "SubmitResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_ResourceUsageId",
                table: "TestResult",
                column: "ResourceUsageId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_TestGroupResultId",
                table: "TestResult",
                column: "TestGroupResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submits");

            migrationBuilder.DropTable(
                name: "TestResult");

            migrationBuilder.DropTable(
                name: "Compilers");

            migrationBuilder.DropTable(
                name: "Solution");

            migrationBuilder.DropTable(
                name: "ResourceUsage");

            migrationBuilder.DropTable(
                name: "TestGroupResult");

            migrationBuilder.DropTable(
                name: "ResourceLimits");

            migrationBuilder.DropTable(
                name: "SubmitResult");

            migrationBuilder.DropTable(
                name: "BuildResult");
        }
    }
}
