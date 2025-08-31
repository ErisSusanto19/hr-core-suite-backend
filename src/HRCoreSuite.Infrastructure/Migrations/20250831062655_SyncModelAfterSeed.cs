using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRCoreSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelAfterSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b5c1848-fdd1-47bd-a3bf-ccb55ef297ef"),
                column: "PasswordHash",
                value: "$2a$11$zeibt7oGO6Gl8YgPRSXaBuvGLGMfexe9DD2fwR9DsvwM0FOVmE0xu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b5c1848-fdd1-47bd-a3bf-ccb55ef297ef"),
                column: "PasswordHash",
                value: "$2a$11$00B6xBQ5JDe6Or7.B6.gzewHfABrfYynr66gdQcRWfvDIN/wiQ7Qa");
        }
    }
}
