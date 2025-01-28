using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MReminders.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoles_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Done = table.Column<bool>(type: "bit", nullable: false),
                    Repeat = table.Column<bool>(type: "bit", nullable: false),
                    Repetition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReminderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Reminders_ReminderId",
                        column: x => x.ReminderId,
                        principalTable: "Reminders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "AppUserId", "ConcurrencyStamp", "CreatedDate", "ModifiedDate", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11526a74-0cd1-4185-b7b2-0113262784e2", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user", "USER" },
                    { "e4e9c144-6f83-4462-9a42-e35b15f97ed4", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0cf41d61-d061-4c1e-8352-5cb789713453", 0, "10486489-a2ea-4772-8ceb-5250afe0e074", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bob@example.com", false, "Bob User", false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BOB@EXAMPLE.COM", "BOB", "AQAAAAIAAYagAAAAEEsJvEz83LWgXVYQYZVpJPn38bgFgh7rGVnyBLJgEeP31LUxcUHQcC8v2E6et5sN/Q==", "+5512987654323", false, "86f618d6-9634-4018-a387-acdc6a08b378", false, "bob" },
                    { "3b395038-e46a-4c82-b4b8-ae2152c841fa", 0, "207dbef8-b245-4090-8d49-139263e7e717", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alice@example.com", false, "Alice User", false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ALICE@EXAMPLE.COM", "ALICE", "AQAAAAIAAYagAAAAEOCy08ywFxE5h0XUvD5z6SlNuqapSTHL+/MgNJMqA1vVmneYzSwaEpaw3HaaG2AvLA==", "+5512987654322", false, "16cc021e-7239-4bd4-8f91-e58ae6632809", false, "alice" },
                    { "44881cc2-d9a2-4d8f-a9d5-07470eb66d52", 0, "a59a511b-e2a1-4856-af02-d8d4f765312d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sysadmin@example.com", false, "System Administrator", false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SYSADMIN@EXAMPLE.COM", "SYSADMIN", "AQAAAAIAAYagAAAAEC0ltfgiUZ6fw+rHJfOyWUiUqXPCCPNlgy/9tqEB3k1r2BY0QYl7WHU2U3JT2LSXhA==", "+5512987654321", false, "1a6dd7a0-820c-47f2-8f68-e962c34f8028", false, "sysadmin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "11526a74-0cd1-4185-b7b2-0113262784e2", "0cf41d61-d061-4c1e-8352-5cb789713453" },
                    { "11526a74-0cd1-4185-b7b2-0113262784e2", "3b395038-e46a-4c82-b4b8-ae2152c841fa" },
                    { "e4e9c144-6f83-4462-9a42-e35b15f97ed4", "44881cc2-d9a2-4d8f-a9d5-07470eb66d52" }
                });

            migrationBuilder.InsertData(
                table: "Reminders",
                columns: new[] { "Id", "CreatedDate", "Description", "Done", "DueDate", "Location", "ModifiedDate", "Name", "Repeat", "Repetition", "UserId" },
                values: new object[,]
                {
                    { "4e9957d7-499d-42f9-a513-15e15767965e", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pay electricity and water bills", false, new DateTime(2025, 1, 30, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4883), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pay Utility Bills", false, "Monthly", "0cf41d61-d061-4c1e-8352-5cb789713453" },
                    { "5019de8a-dc3c-4699-8410-ec54b4c6a15d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Participate in weekly meeting", false, new DateTime(2025, 2, 1, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4862), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Weekly Meeting", false, "Weekly", "3b395038-e46a-4c82-b4b8-ae2152c841fa" },
                    { "937a8fa6-c14b-4e17-a4cc-16aee9f7b322", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Have lunch with the team", false, new DateTime(2025, 2, 8, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4899), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Team Lunch", false, "None", "3b395038-e46a-4c82-b4b8-ae2152c841fa" },
                    { "a6a2cdac-72c5-4447-aef0-6fc29cb42e95", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Perform daily backup", false, new DateTime(2025, 1, 26, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4316), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Daily Backup", false, "Daily", "44881cc2-d9a2-4d8f-a9d5-07470eb66d52" },
                    { "bbee338d-1dc6-4309-a859-e34b5236256d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Buy groceries for the week", false, new DateTime(2025, 1, 28, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4874), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Grocery Shopping", false, "None", "0cf41d61-d061-4c1e-8352-5cb789713453" }
                });

            migrationBuilder.InsertData(
                table: "Attachments",
                columns: new[] { "Id", "Content", "ContentType", "CreatedDate", "FileName", "ModifiedDate", "ReminderId" },
                values: new object[,]
                {
                    { "7390b5e4-ab20-4a05-adf1-a1ade898052d", new byte[0], "text/x-powershell", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BackupAutomationScript.ps1", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "a6a2cdac-72c5-4447-aef0-6fc29cb42e95" },
                    { "fb6a88c0-5f7f-4782-acec-bd2c2e7a584b", new byte[0], "application/pdf", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "GroceryList.pdf", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bbee338d-1dc6-4309-a859-e34b5236256d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_AppUserId",
                table: "AspNetRoles",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ReminderId_FileName",
                table: "Attachments",
                columns: new[] { "ReminderId", "FileName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId_Name",
                table: "Reminders",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
