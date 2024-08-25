using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _2024FinalYearProject.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4fe9c6d4-b25c-4d5c-9a15-7edb3cee631a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "66c45ef7-7060-4327-a96d-a789829ad97a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "854fa539-c08f-4edb-a63b-3d6680857ce0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2cc7ad2-7b45-4349-955e-cf078161ec49");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e0670559-1ef0-48db-a7fe-bae0926769c9");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1068859b-97bc-4311-94bc-aff1c54d258e", null, "User", "USER" },
                    { "2cf2d3b2-471f-4e6d-998b-3aed844a4ec0", null, "Admin", "ADMIN" },
                    { "8c294ba4-7b48-40f2-874b-f88f71bc0d4a", null, "Consultant", "CONSULTANT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1068859b-97bc-4311-94bc-aff1c54d258e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2cf2d3b2-471f-4e6d-998b-3aed844a4ec0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c294ba4-7b48-40f2-874b-f88f71bc0d4a");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4fe9c6d4-b25c-4d5c-9a15-7edb3cee631a", null, "Student", "STUDENT" },
                    { "66c45ef7-7060-4327-a96d-a789829ad97a", null, "Consultant", "CONSULTANT" },
                    { "854fa539-c08f-4edb-a63b-3d6680857ce0", null, "Admin", "ADMIN" },
                    { "c2cc7ad2-7b45-4349-955e-cf078161ec49", null, "User", "USER" },
                    { "e0670559-1ef0-48db-a7fe-bae0926769c9", null, "Staff", "STAFF" }
                });
        }
    }
}
