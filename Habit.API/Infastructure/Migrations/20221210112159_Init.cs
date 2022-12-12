using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Habit.API.Infastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "habit_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "notification_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "periodicity_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Periodicity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periodicity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PeriodicityId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    CompletedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habits_Periodicity_PeriodicityId",
                        column: x => x.PeriodicityId,
                        principalTable: "Periodicity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Periodicity",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 1, "everyday", "Каждый день" });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_PeriodicityId",
                table: "Habits",
                column: "PeriodicityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_HabitId",
                table: "Notification",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_Periodicity_Code",
                table: "Periodicity",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Habits");

            migrationBuilder.DropTable(
                name: "Periodicity");

            migrationBuilder.DropSequence(
                name: "habit_hilo");

            migrationBuilder.DropSequence(
                name: "notification_hilo");

            migrationBuilder.DropSequence(
                name: "periodicity_hilo");
        }
    }
}
