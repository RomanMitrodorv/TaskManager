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

            migrationBuilder.CreateSequence(
                name: "task_status_hilo",
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
                name: "TaskStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => x.Id);
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
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    LabelId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Tasks_TaskStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TaskLabel",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "Work", "Работа" },
                    { 2, "Home", "Дома" }
                });

            migrationBuilder.InsertData(
                table: "TaskStatus",
                columns: new[] { "Id", "Code", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "doing", "В работе", 20 },
                    { 3, "created", "Создано", 10 },
                    { 4, "completed", "Выполнено", 30 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskLabel_Code",
                table: "TaskLabel",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_LabelId",
                table: "Tasks",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatus_Code",
                table: "TaskStatus",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskLabel");

            migrationBuilder.DropTable(
                name: "TaskStatus");

            migrationBuilder.DropSequence(
                name: "scheduled_task_hilo");

            migrationBuilder.DropSequence(
                name: "task_label_hilo");

            migrationBuilder.DropSequence(
                name: "task_status_hilo");
        }
    }
}
