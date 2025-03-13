using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChartImageToChartModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChartImage",
                table: "Charts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartImage",
                table: "Charts");
        }
    }
}
