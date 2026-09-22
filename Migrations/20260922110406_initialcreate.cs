using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "EmployeeIdSequence");

            migrationBuilder.CreateTable(
                name: "Leavetypes",
                columns: table => new
                {
                    leavetypeid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    leavetypename = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    maximumdays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leavetypes", x => x.leavetypeid);
                });

            migrationBuilder.CreateTable(
                name: "Loginuser",
                columns: table => new
                {
                    employeeid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    passwordhash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loginuser", x => x.employeeid);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Loginuser_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Loginuser",
                        principalColumn: "employeeid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leavebalances",
                columns: table => new
                {
                    leavebalanceid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeeid = table.Column<int>(type: "int", nullable: false),
                    leavetypeid = table.Column<int>(type: "int", nullable: false),
                    totaldays = table.Column<int>(type: "int", nullable: false),
                    useddays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leavebalances", x => x.leavebalanceid);
                    table.ForeignKey(
                        name: "FK_Leavebalances_Employees_employeeid",
                        column: x => x.employeeid,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leavebalances_Leavetypes_leavetypeid",
                        column: x => x.leavetypeid,
                        principalTable: "Leavetypes",
                        principalColumn: "leavetypeid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leaverequests",
                columns: table => new
                {
                    leaverequestid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeeid = table.Column<int>(type: "int", nullable: false),
                    leavetypeid = table.Column<int>(type: "int", nullable: false),
                    fromdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    todate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    applieddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    approvedby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaverequests", x => x.leaverequestid);
                    table.ForeignKey(
                        name: "FK_Leaverequests_Employees_employeeid",
                        column: x => x.employeeid,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leaverequests_Leavetypes_leavetypeid",
                        column: x => x.leavetypeid,
                        principalTable: "Leavetypes",
                        principalColumn: "leavetypeid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leavebalances_employeeid_leavetypeid",
                table: "Leavebalances",
                columns: new[] { "employeeid", "leavetypeid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leavebalances_leavetypeid",
                table: "Leavebalances",
                column: "leavetypeid");

            migrationBuilder.CreateIndex(
                name: "IX_Leaverequests_employeeid",
                table: "Leaverequests",
                column: "employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_Leaverequests_leavetypeid",
                table: "Leaverequests",
                column: "leavetypeid");

            migrationBuilder.CreateIndex(
                name: "IX_Leavetypes_leavetypename",
                table: "Leavetypes",
                column: "leavetypename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loginuser_username",
                table: "Loginuser",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leavebalances");

            migrationBuilder.DropTable(
                name: "Leaverequests");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Leavetypes");

            migrationBuilder.DropTable(
                name: "Loginuser");

            migrationBuilder.DropSequence(
                name: "EmployeeIdSequence");
        }
    }
}
