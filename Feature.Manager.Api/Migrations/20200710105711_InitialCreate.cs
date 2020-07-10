using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Feature.Manager.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeatureRuns",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Allocation = table.Column<int>(nullable: false),
                    FeatId = table.Column<string>(nullable: false),
                    StartAt = table.Column<DateTime>(nullable: false),
                    EndAt = table.Column<DateTime>(nullable: true),
                    RunToken = table.Column<string>(nullable: false),
                    StopResult = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Hypothesis = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    FeatId = table.Column<string>(nullable: false),
                    ExperimentToken = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureRuns_EndAt",
                table: "FeatureRuns",
                column: "EndAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureRuns_RunToken",
                table: "FeatureRuns",
                column: "RunToken");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureRuns_StartAt",
                table: "FeatureRuns",
                column: "StartAt");

            migrationBuilder.CreateIndex(
                name: "IX_Features_FeatId",
                table: "Features",
                column: "FeatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeatureRuns");

            migrationBuilder.DropTable(
                name: "Features");
        }
    }
}
