using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monitoring.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedandentKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_Analytics_AnalyticsId",
                table: "Websites");

            migrationBuilder.DropIndex(
                name: "IX_Websites_AnalyticsId",
                table: "Websites");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_WebsiteId",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "AnalyticsId",
                table: "Websites");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_WebsiteId",
                table: "Analytics",
                column: "WebsiteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Analytics_WebsiteId",
                table: "Analytics");

            migrationBuilder.AddColumn<int>(
                name: "AnalyticsId",
                table: "Websites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Websites_AnalyticsId",
                table: "Websites",
                column: "AnalyticsId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_WebsiteId",
                table: "Analytics",
                column: "WebsiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_Analytics_AnalyticsId",
                table: "Websites",
                column: "AnalyticsId",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
