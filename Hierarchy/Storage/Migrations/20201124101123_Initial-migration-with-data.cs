using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hierarchy.Storage.Migrations
{
    public partial class Initialmigrationwithdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "HierarchyId", "Name", "OldHierarchyId" },
                values: new object[,]
                {
                    { 1, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/"), "Lidia Brewer", null },
                    { 2, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/2/"), "Hannah Wicks", null },
                    { 3, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/1/"), "Sheridan Perkins", null },
                    { 4, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/2/"), "Zakaria Bailey", null },
                    { 5, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/2/1/"), "Albert Woodward", null },
                    { 6, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/2/2/"), "Arron Mcdaniel", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
