using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Database.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260801183002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "api");

            migrationBuilder.EnsureSchema(
                name: "log");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "ApiLogs",
                schema: "api",
                columns: table => new
                {
                    ApiLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    TraceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    QueryString = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    RequestHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogs", x => x.ApiLogId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationLogs",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "identity",
                columns: table => new
                {
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    PermissionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ParentPermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_Permissions_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Permissions_Parent",
                        column: x => x.ParentPermissionId,
                        principalSchema: "identity",
                        principalTable: "Permissions",
                        principalColumn: "PermissionId");
                    table.ForeignKey(
                        name: "FK_Permissions_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "identity",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Roles_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Roles_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                schema: "identity",
                columns: table => new
                {
                    UserPermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.UserPermissionId);
                    table.ForeignKey(
                        name: "FK_UserPermissions_AssignedBy",
                        column: x => x.AssignedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_UserPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "identity",
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "identity",
                columns: table => new
                {
                    RolePermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.RolePermissionId);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AssignedBy",
                        column: x => x.AssignedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_RolePermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "identity",
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "identity",
                columns: table => new
                {
                    UserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "FK_UserRoles_AssignedBy",
                        column: x => x.AssignedBy,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_CreatedOn",
                schema: "api",
                table: "ApiLogs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_TraceId",
                schema: "api",
                table: "ApiLogs",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_UserId",
                schema: "api",
                table: "ApiLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Category",
                schema: "identity",
                table: "Permissions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                schema: "identity",
                table: "Permissions",
                column: "PermissionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedBy",
                schema: "identity",
                table: "Permissions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Parent",
                schema: "identity",
                table: "Permissions",
                column: "ParentPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UpdatedBy",
                schema: "identity",
                table: "Permissions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UQ_Permissions_Code",
                schema: "identity",
                table: "Permissions",
                column: "PermissionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_AssignedBy",
                schema: "identity",
                table: "RolePermissions",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "identity",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "identity",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ_RolePermissions",
                schema: "identity",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedBy",
                schema: "identity",
                table: "Roles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UpdatedBy",
                schema: "identity",
                table: "Roles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UQ_Roles_RoleName",
                schema: "identity",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_AssignedBy",
                schema: "identity",
                table: "UserPermissions",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                schema: "identity",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                schema: "identity",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserPermissions",
                schema: "identity",
                table: "UserPermissions",
                columns: new[] { "UserId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedBy",
                schema: "identity",
                table: "UserRoles",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "identity",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserRoles_UserId_RoleId",
                schema: "identity",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                schema: "identity",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedBy",
                schema: "identity",
                table: "Users",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "identity",
                table: "Users",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "UQ_Users_Email",
                schema: "identity",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Users_Username",
                schema: "identity",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLogs",
                schema: "api");

            migrationBuilder.DropTable(
                name: "ApplicationLogs",
                schema: "log");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserPermissions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "identity");
        }
    }
}
