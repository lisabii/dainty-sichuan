using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Huihuibao.Data.Migrations
{
    public partial class changeApproToAssign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Approved",
                table: "TimeTables",
                newName: "Assigned");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Assigned",
                table: "TimeTables",
                newName: "Arranged");
        }
    }
}
