using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.API.Infastructure.Migrations.TaskDb
{
    public partial class AddSortOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "TaskStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemIndex",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "SortOrder",
                value: 20);

            migrationBuilder.UpdateData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "SortOrder",
                value: 10);

            migrationBuilder.UpdateData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 4,
                column: "SortOrder",
                value: 30);

            migrationBuilder.UpdateData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 5,
                column: "SortOrder",
                value: 40);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "ItemIndex",
                table: "Tasks");

            migrationBuilder.InsertData(
                table: "TaskStatus",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 2, "blocked", "Заблакировано" });
        }
    }
}
