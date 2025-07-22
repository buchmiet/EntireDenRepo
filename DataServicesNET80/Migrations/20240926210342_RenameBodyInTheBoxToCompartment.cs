using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataServicesNET80.Migrations
{
    /// <inheritdoc />
    public partial class RenameBodyInTheBoxToCompartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "row",
                table: "bodyinthebox",
                newName: "Row");

            migrationBuilder.RenameColumn(
                name: "itembodyID",
                table: "bodyinthebox",
                newName: "ItemBodyId");

            migrationBuilder.RenameColumn(
                name: "column",
                table: "bodyinthebox",
                newName: "Column");

            migrationBuilder.RenameColumn(
                name: "BodyInTheBoxID",
                table: "bodyinthebox",
                newName: "CompartmentId");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Row",
                table: "bodyinthebox",
                newName: "row");

            migrationBuilder.RenameColumn(
                name: "ItemBodyId",
                table: "bodyinthebox",
                newName: "itembodyID");

            migrationBuilder.RenameColumn(
                name: "Column",
                table: "bodyinthebox",
                newName: "column");

            migrationBuilder.RenameColumn(
                name: "CompartmentId",
                table: "bodyinthebox",
                newName: "BodyInTheBoxID");

         
        }
    }
}
