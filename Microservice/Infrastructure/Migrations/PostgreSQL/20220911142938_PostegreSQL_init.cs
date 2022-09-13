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

            migrationBuilder.CreateTable(
                name: "users_roles",
                schema: "web",
                columns: table => new
                {
                    username = table.Column<string>(type: "CHAR(30)", maxLength: 30, nullable: false),
                    rolename = table.Column<string>(type: "CHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_roles", x => new { x.username, x.rolename });
                    table.ForeignKey(
                        name: "FK_users_roles_roles_rolename",
                        column: x => x.rolename,
                        principalSchema: "web",
                        principalTable: "roles",
                        principalColumn: "rolename",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_roles_users_username",
                        column: x => x.username,
                        principalSchema: "web",
                        principalTable: "users",
                        principalColumn: "username",
                        onDelete: ReferentialAction.Cascade);
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
                values: new object[] { "Tom96", "tommaso_zazzaretti_96@gmail.com", "Tommaso", "gRkO6op4MmcN9EyzigGoBg==.vYKv8wyKaHQan7rrG4Bf3L656s+h/+if3v35Ypn9O6o=", "Zazzaretti" });

            migrationBuilder.InsertData(
                schema: "web",
                table: "users_roles",
                columns: new[] { "rolename", "username" },
                values: new object[,]
                {
                    { "ADMIN", "Tom96" },
                    { "SUPER-ADMIN", "Tom96" },
                    { "USER", "Tom96" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_users_roles_rolename",
                schema: "web",
                table: "users_roles",
                column: "rolename");

            migrationBuilder.CreateIndex(
                name: "IX_users_roles_username",
                schema: "web",
                table: "users_roles",
                column: "username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users_roles",
                schema: "web");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "web");

            migrationBuilder.DropTable(
                name: "users",
                schema: "web");
        }
    }
}
