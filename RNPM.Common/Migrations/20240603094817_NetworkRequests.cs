using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RNPM.Common.Migrations
{
    /// <inheritdoc />
    public partial class NetworkRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HttpRequestInstances_HttpRequests_RequestId",
                table: "HttpRequestInstances");

            migrationBuilder.DropTable(
                name: "HttpRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Navigations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Navigations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Navigations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Navigations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Navigations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Navigations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Navigations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "NetworkRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkRequests_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkRequests_ApplicationId",
                table: "NetworkRequests",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpRequestInstances_NetworkRequests_RequestId",
                table: "HttpRequestInstances",
                column: "RequestId",
                principalTable: "NetworkRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HttpRequestInstances_NetworkRequests_RequestId",
                table: "HttpRequestInstances");

            migrationBuilder.DropTable(
                name: "NetworkRequests");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Navigations");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Navigations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Navigations");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Navigations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Navigations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Navigations");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Navigations");

            migrationBuilder.CreateTable(
                name: "HttpRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpRequests_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequests_ApplicationId",
                table: "HttpRequests",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpRequestInstances_HttpRequests_RequestId",
                table: "HttpRequestInstances",
                column: "RequestId",
                principalTable: "HttpRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
