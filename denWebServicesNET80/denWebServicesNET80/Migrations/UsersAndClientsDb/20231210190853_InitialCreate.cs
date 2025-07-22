#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace denWebServicesNET80.Migrations.UsersAndClientsDb;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LoggedInClients",
            columns: table => new
            {
                LoggedInClientId = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                UserClientNamesId = table.Column<int>(type: "INTEGER", nullable: false),
                Handshake = table.Column<string>(type: "TEXT", nullable: false),
                IsConnected = table.Column<bool>(type: "INTEGER", nullable: false),
                TimeCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                ConnectionId = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LoggedInClients", x => x.LoggedInClientId);
            });

        migrationBuilder.CreateTable(
            name: "UserClientNamess",
            columns: table => new
            {
                UserClientNamesId = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                UserId = table.Column<int>(type: "INTEGER", nullable: false),
                ClientName = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserClientNamess", x => x.UserClientNamesId);
            });

        migrationBuilder.CreateTable(
            name: "UserMaxClientsAssociations",
            columns: table => new
            {
                UserId = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                UserName = table.Column<string>(type: "TEXT", nullable: false),
                MaxClients = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserMaxClientsAssociations", x => x.UserId);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserClientNamess_UserId_ClientName",
            table: "UserClientNamess",
            columns: new[] { "UserId", "ClientName" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LoggedInClients");

        migrationBuilder.DropTable(
            name: "UserClientNamess");

        migrationBuilder.DropTable(
            name: "UserMaxClientsAssociations");
    }
}