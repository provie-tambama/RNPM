using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RNPM.Common.Migrations
{
    /// <inheritdoc />
    public partial class _2025_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HttpRequestInstances_NetworkRequests_RequestId",
                table: "HttpRequestInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_NavigationInstances_Navigations_NavigationId",
                table: "NavigationInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_NetworkRequests_Applications_ApplicationId",
                table: "NetworkRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ScreenComponentRenders_ScreenComponents_ComponentId",
                table: "ScreenComponentRenders");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ScreenComponents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "OptimizationSuggestion",
                table: "ScreenComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceCode",
                table: "ScreenComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ComponentId",
                table: "ScreenComponentRenders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationId",
                table: "NetworkRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ToScreen",
                table: "Navigations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FromScreen",
                table: "Navigations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NavigationId",
                table: "NavigationInstances",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestId",
                table: "HttpRequestInstances",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TokenValidity",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Fullname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UniqueAccessCode",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "OptimizationSuggestions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ComponentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OptimizationSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptimizationSuggestions_ScreenComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "ScreenComponents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptimizationSuggestions_ComponentId",
                table: "OptimizationSuggestions",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpRequestInstances_NetworkRequests_RequestId",
                table: "HttpRequestInstances",
                column: "RequestId",
                principalTable: "NetworkRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NavigationInstances_Navigations_NavigationId",
                table: "NavigationInstances",
                column: "NavigationId",
                principalTable: "Navigations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NetworkRequests_Applications_ApplicationId",
                table: "NetworkRequests",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenComponentRenders_ScreenComponents_ComponentId",
                table: "ScreenComponentRenders",
                column: "ComponentId",
                principalTable: "ScreenComponents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HttpRequestInstances_NetworkRequests_RequestId",
                table: "HttpRequestInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_NavigationInstances_Navigations_NavigationId",
                table: "NavigationInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_NetworkRequests_Applications_ApplicationId",
                table: "NetworkRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ScreenComponentRenders_ScreenComponents_ComponentId",
                table: "ScreenComponentRenders");

            migrationBuilder.DropTable(
                name: "OptimizationSuggestions");

            migrationBuilder.DropColumn(
                name: "OptimizationSuggestion",
                table: "ScreenComponents");

            migrationBuilder.DropColumn(
                name: "SourceCode",
                table: "ScreenComponents");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ScreenComponents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ComponentId",
                table: "ScreenComponentRenders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationId",
                table: "NetworkRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ToScreen",
                table: "Navigations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FromScreen",
                table: "Navigations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NavigationId",
                table: "NavigationInstances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestId",
                table: "HttpRequestInstances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TokenValidity",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Fullname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UniqueAccessCode",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HttpRequestInstances_NetworkRequests_RequestId",
                table: "HttpRequestInstances",
                column: "RequestId",
                principalTable: "NetworkRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NavigationInstances_Navigations_NavigationId",
                table: "NavigationInstances",
                column: "NavigationId",
                principalTable: "Navigations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NetworkRequests_Applications_ApplicationId",
                table: "NetworkRequests",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenComponentRenders_ScreenComponents_ComponentId",
                table: "ScreenComponentRenders",
                column: "ComponentId",
                principalTable: "ScreenComponents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
