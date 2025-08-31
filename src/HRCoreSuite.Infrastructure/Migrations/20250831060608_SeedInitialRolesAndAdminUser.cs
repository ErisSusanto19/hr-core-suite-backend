using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRCoreSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialRolesAndAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), "HR_Admin" },
                    { new Guid("acc5645b-f080-4011-8c98-d7efa3032f52"), "Super_Admin" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EmployeeId", "PasswordHash", "UserName" },
                values: new object[] { new Guid("0b5c1848-fdd1-47bd-a3bf-ccb55ef297ef"), "admin@hrcoresuite.com", null, "$2a$11$00B6xBQ5JDe6Or7.B6.gzewHfABrfYynr66gdQcRWfvDIN/wiQ7Qa", "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("acc5645b-f080-4011-8c98-d7efa3032f52"), new Guid("0b5c1848-fdd1-47bd-a3bf-ccb55ef297ef") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("acc5645b-f080-4011-8c98-d7efa3032f52"), new Guid("0b5c1848-fdd1-47bd-a3bf-ccb55ef297ef") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("acc5645b-f080-4011-8c98-d7efa3032f52"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b5c1848-fdd1-47bd-a3bf-ccb55ef297ef"));
        }
    }
}
