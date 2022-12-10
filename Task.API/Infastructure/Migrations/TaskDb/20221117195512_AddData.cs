using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.API.Infastructure.Migrations.TaskDb
{
    public partial class AddData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TaskStatus",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 4, "completed", "Выполнено" });

            migrationBuilder.InsertData(
                table: "TaskStatus",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 5, "future", "В плане" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
