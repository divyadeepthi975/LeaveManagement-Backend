using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Migrations
{
    /// <inheritdoc />
    public partial class leavetype_leaverequest_leavebalance_tables_created : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leavetypes",
                columns: table => new
                {
                    leavetypeid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    leavetypename = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    maximumdays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leavetypes", x => x.leavetypeid);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leavebalances");

            migrationBuilder.DropTable(
                name: "Leaverequests");

            migrationBuilder.DropTable(
                name: "Leavetypes");
        }
    }
}
