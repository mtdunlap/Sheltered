using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;


#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "Generated Code")]
    public partial class AddAnimalSex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sex",
                schema: "sheltered",
                table: "animals",
                type: "TEXT",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sex",
                schema: "sheltered",
                table: "animals");
        }
    }
}
