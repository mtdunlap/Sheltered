using System;
using Core.Animals;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sheltered");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:sheltered.animal_kind", "unspecified,dog,cat");

            migrationBuilder.CreateTable(
                name: "animals",
                schema: "sheltered",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 50, nullable: true),
                    kind = table.Column<AnimalKind>(type: "sheltered.animal_kind", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_animals", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "animals",
                schema: "sheltered");
        }
    }
}
