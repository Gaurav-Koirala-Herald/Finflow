using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoleBaseAuthorization.Migrations
{
    /// <inheritdoc />
    public partial class SecondFixedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Functions_Modules_ModuleId",
                table: "Functions");

            migrationBuilder.DropForeignKey(
                name: "FK_Privileges_Modules_ModuleId",
                table: "Privileges");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleFunctions_Functions_FunctionId",
                table: "RoleFunctions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleFunctions_Roles_RoleId",
                table: "RoleFunctions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId",
                table: "RolePrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePrivileges_Roles_RoleId",
                table: "RolePrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_Functions_Modules_ModuleId",
                table: "Functions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Privileges_Modules_ModuleId",
                table: "Privileges",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleFunctions_Functions_FunctionId",
                table: "RoleFunctions",
                column: "FunctionId",
                principalTable: "Functions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleFunctions_Roles_RoleId",
                table: "RoleFunctions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId",
                table: "RolePrivileges",
                column: "PrivilegeId",
                principalTable: "Privileges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePrivileges_Roles_RoleId",
                table: "RolePrivileges",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Functions_Modules_ModuleId",
                table: "Functions");

            migrationBuilder.DropForeignKey(
                name: "FK_Privileges_Modules_ModuleId",
                table: "Privileges");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleFunctions_Functions_FunctionId",
                table: "RoleFunctions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleFunctions_Roles_RoleId",
                table: "RoleFunctions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId",
                table: "RolePrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePrivileges_Roles_RoleId",
                table: "RolePrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_Functions_Modules_ModuleId",
                table: "Functions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Privileges_Modules_ModuleId",
                table: "Privileges",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleFunctions_Functions_FunctionId",
                table: "RoleFunctions",
                column: "FunctionId",
                principalTable: "Functions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleFunctions_Roles_RoleId",
                table: "RoleFunctions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId",
                table: "RolePrivileges",
                column: "PrivilegeId",
                principalTable: "Privileges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePrivileges_Roles_RoleId",
                table: "RolePrivileges",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
