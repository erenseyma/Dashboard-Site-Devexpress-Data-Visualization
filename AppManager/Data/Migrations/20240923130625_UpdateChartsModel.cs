using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChartsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DatabaseId",
                table: "Charts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatabaseId",
                table: "Charts");
        }
    }
}
