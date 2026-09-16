using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Migrations
{
    /// <inheritdoc />
    public partial class add_Loginusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loginuser",
                columns: table => new
                {
                    loginuserid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeeid = table.Column<int>(type: "int", nullable: false),
                    username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    passwordhash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loginuser", x => x.loginuserid);
                    table.ForeignKey(
                        name: "FK_Loginuser_Employees_employeeid",
                        column: x => x.employeeid,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loginuser_employeeid",
                table: "Loginuser",
                column: "employeeid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loginuser");
        }
    }
}
