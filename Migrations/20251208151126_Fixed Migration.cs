using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoleBaseAuthorization.Migrations
{
    /// <inheritdoc />
    public partial class FixedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "Admin@123");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$rBNhFjv.VH8QP8IB9DLRAO2CsOFfIXjY1v8pD9/Hw5rk3QdKmjfHa");
        }
    }
}
