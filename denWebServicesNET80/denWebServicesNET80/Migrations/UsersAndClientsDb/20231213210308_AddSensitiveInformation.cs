#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace denWebServicesNET80.Migrations.UsersAndClientsDb;

/// <inheritdoc />
public partial class AddSensitiveInformation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SensitiveInformations",
            columns: table => new
            {
                SensitiveInformationId = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                UserId = table.Column<int>(type: "INTEGER", nullable: false),
                DBHost = table.Column<string>(type: "TEXT", nullable: false),
                DBPort = table.Column<string>(type: "TEXT", nullable: false),
                DBUserName = table.Column<string>(type: "TEXT", nullable: false),
                DBPassword = table.Column<string>(type: "TEXT", nullable: false),
                DBname = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SensitiveInformations", x => x.SensitiveInformationId);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SensitiveInformations");
    }
}