using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microservice.Infrastructure.Migrations.PostgreSQL
{
    public partial class PostegreSQL_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "web");

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "web",
                columns: table => new
                {
                    rolename = table.Column<string>(type: "CHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.rolename);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "web",
                columns: table => new
                {
                    username = table.Column<string>(type: "CHAR(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "CHAR(50)", maxLength: 50, nullable: false),
                    surname = table.Column<string>(type: "CHAR(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "CHAR(80)", maxLength: 80, nullable: false),
                    password = table.Column<string>(type: "CHAR(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.username);
                });

            migrationBuilder.InsertData(
                schema: "web",
                table: "roles",
                column: "rolename",
                values: new object[]
                {
                    "ADMIN",
                    "SUPER-ADMIN",
                    "USER"
                });

            migrationBuilder.InsertData(
                schema: "web",
                table: "users",
                columns: new[] { "username", "email", "name", "password", "surname" },
                values: new object[] { "Tom96", "tommaso.zazzaretti96@gmail.com", "Tommaso", "P@ssw0rd", "Zazzaretti" });

            migrationBuilder.CreateIndex(
                name: "IX_roles_rolename",
                schema: "web",
                table: "roles",
                column: "rolename");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "web",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                schema: "web",
                table: "users",
                column: "username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roles",
                schema: "web");

            migrationBuilder.DropTable(
                name: "users",
                schema: "web");
        }
    }
}
