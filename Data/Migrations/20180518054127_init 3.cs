using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Huihuibao.Data.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeTable_AspNetUsers_ApplicationUserId",
                table: "TimeTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeTable",
                table: "TimeTable");

            migrationBuilder.RenameTable(
                name: "TimeTable",
                newName: "TimeTables");

            migrationBuilder.RenameIndex(
                name: "IX_TimeTable_ApplicationUserId",
                table: "TimeTables",
                newName: "IX_TimeTables_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeTables",
                table: "TimeTables",
                column: "TimeTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeTables_AspNetUsers_ApplicationUserId",
                table: "TimeTables",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeTables_AspNetUsers_ApplicationUserId",
                table: "TimeTables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeTables",
                table: "TimeTables");

            migrationBuilder.RenameTable(
                name: "TimeTables",
                newName: "TimeTable");

            migrationBuilder.RenameIndex(
                name: "IX_TimeTables_ApplicationUserId",
                table: "TimeTable",
                newName: "IX_TimeTable_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeTable",
                table: "TimeTable",
                column: "TimeTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeTable_AspNetUsers_ApplicationUserId",
                table: "TimeTable",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
