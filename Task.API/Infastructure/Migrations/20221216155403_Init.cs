using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.API.Infastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "scheduled_task_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "task_label_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "TaskLabel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLabel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LabelId = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    ItemIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskLabel_LabelId",
                        column: x => x.LabelId,
                        principalTable: "TaskLabel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TaskLabel",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 1, "Work", "Работа" });

            migrationBuilder.InsertData(
                table: "TaskLabel",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 2, "Home", "Дома" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskLabel_Code",
                table: "TaskLabel",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_LabelId",
                table: "Tasks",
                column: "LabelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskLabel");

            migrationBuilder.DropSequence(
                name: "scheduled_task_hilo");

            migrationBuilder.DropSequence(
                name: "task_label_hilo");
        }
    }
}
