using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hierarchy.Storage.Migrations
{
    public partial class UniqueconstraintonhierarchyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<HierarchyId>(
                name: "HierarchyId",
                table: "Employee",
                type: "hierarchyid",
                nullable: false,
                oldClrType: typeof(HierarchyId),
                oldType: "hierarchyid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_HierarchyId",
                table: "Employee",
                column: "HierarchyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employee_HierarchyId",
                table: "Employee");

            migrationBuilder.AlterColumn<HierarchyId>(
                name: "HierarchyId",
                table: "Employee",
                type: "hierarchyid",
                nullable: true,
                oldClrType: typeof(HierarchyId),
                oldType: "hierarchyid");
        }
    }
}
