using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZetaFin.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPreRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts");

            migrationBuilder.AlterColumn<string>(
                name: "WhatsAppNumber",
                table: "UserWhatsApps",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserWhatsApps",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMessageAt",
                table: "UserWhatsApps",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "UserWhatsApps",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "UserWhatsApps",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "UserWhatsApps",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedUntil",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GoalId1",
                table: "UserGoals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CustomMonthlyTarget",
                table: "UserGoals",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GoalId",
                table: "UserGoals",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserGoals",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptUrl",
                table: "Transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptOcrData",
                table: "Transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasReceipt",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ExpenseType",
                table: "Transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Transactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Receipts",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Receipts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionId",
                table: "Receipts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OcrProcessed",
                table: "Receipts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            // Conversão explícita de text para jsonb
            migrationBuilder.Sql(
                "ALTER TABLE \"Receipts\" ALTER COLUMN \"OcrDataJson\" TYPE jsonb USING \"OcrDataJson\"::jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "Receipts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "Receipts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "Receipts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Receipts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Receipts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Receipts",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TargetDate",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TargetAmount",
                table: "Goals",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Goals",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentAmount",
                table: "Goals",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Goals",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Expenses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Expenses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Expenses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Deposits",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Deposits",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "GoalId",
                table: "Deposits",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Deposits",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Deposits",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Deposits",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Resource = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pre_registrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    whatsapp = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    faixa_etaria = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    aceitou_lgpd = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    origem_lead = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    convertido_para_usuario = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    data_conversao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pre_registrations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokeReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAccessAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TerminatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshTokenId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Type",
                table: "Transactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts",
                column: "TransactionId",
                unique: true,
                filter: "\"TransactionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_CreatedAt",
                table: "Goals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Date",
                table: "Expenses",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_Date",
                table: "Deposits",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_pre_reg_whatsapp",
                table: "pre_registrations",
                column: "whatsapp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_RevokedAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_IsActive",
                table: "UserSessions",
                columns: new[] { "UserId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AuditLogs");
            migrationBuilder.DropTable(name: "pre_registrations");
            migrationBuilder.DropTable(name: "RefreshTokens");
            migrationBuilder.DropTable(name: "UserSessions");

            migrationBuilder.DropIndex(name: "IX_Users_CreatedAt", table: "Users");
            migrationBuilder.DropIndex(name: "IX_Users_Email", table: "Users");
            migrationBuilder.DropIndex(name: "IX_Transactions_Type", table: "Transactions");
            migrationBuilder.DropIndex(name: "IX_Receipts_TransactionId", table: "Receipts");
            migrationBuilder.DropIndex(name: "IX_Goals_CreatedAt", table: "Goals");
            migrationBuilder.DropIndex(name: "IX_Expenses_Date", table: "Expenses");
            migrationBuilder.DropIndex(name: "IX_Expenses_UserId", table: "Expenses");
            migrationBuilder.DropIndex(name: "IX_Deposits_Date", table: "Deposits");

            migrationBuilder.DropColumn(name: "CreatedAt", table: "Users");
            migrationBuilder.DropColumn(name: "FailedLoginAttempts", table: "Users");
            migrationBuilder.DropColumn(name: "IsActive", table: "Users");
            migrationBuilder.DropColumn(name: "LastLoginAt", table: "Users");
            migrationBuilder.DropColumn(name: "LockedUntil", table: "Users");

            migrationBuilder.AlterColumn<string>(name: "WhatsAppNumber", table: "UserWhatsApps", type: "TEXT", maxLength: 20, nullable: false, oldClrType: typeof(string), oldType: "character varying(20)", oldMaxLength: 20);
            migrationBuilder.AlterColumn<string>(name: "UserId", table: "UserWhatsApps", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "LastMessageAt", table: "UserWhatsApps", type: "TEXT", nullable: true, oldClrType: typeof(DateTime), oldType: "timestamp with time zone", oldNullable: true);
            migrationBuilder.AlterColumn<int>(name: "IsActive", table: "UserWhatsApps", type: "INTEGER", nullable: false, oldClrType: typeof(bool), oldType: "boolean");
            migrationBuilder.AlterColumn<string>(name: "CreatedAt", table: "UserWhatsApps", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "UserWhatsApps", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "Role", table: "Users", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "character varying(50)", oldMaxLength: 50);
            migrationBuilder.AlterColumn<string>(name: "PasswordHash", table: "Users", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "text");
            migrationBuilder.AlterColumn<string>(name: "Name", table: "Users", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "character varying(200)", oldMaxLength: 200);
            migrationBuilder.AlterColumn<int>(name: "IsEmailConfirmed", table: "Users", type: "INTEGER", nullable: false, oldClrType: typeof(bool), oldType: "boolean");
            migrationBuilder.AlterColumn<string>(name: "Email", table: "Users", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "character varying(255)", oldMaxLength: 255);
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Users", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "GoalId1", table: "UserGoals", type: "TEXT", nullable: true, oldClrType: typeof(Guid), oldType: "uuid", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "CustomMonthlyTarget", table: "UserGoals", type: "TEXT", nullable: true, oldClrType: typeof(decimal), oldType: "numeric(18,2)", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "GoalId", table: "UserGoals", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "UserId", table: "UserGoals", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "UserId", table: "Transactions", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "UpdatedAt", table: "Transactions", type: "TEXT", nullable: true, oldClrType: typeof(DateTime), oldType: "timestamp with time zone", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "Type", table: "Transactions", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "text");
            migrationBuilder.AlterColumn<string>(name: "ReceiptUrl", table: "Transactions", type: "TEXT", nullable: true, oldClrType: typeof(string), oldType: "text", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "ReceiptOcrData", table: "Transactions", type: "TEXT", nullable: true, oldClrType: typeof(string), oldType: "text", oldNullable: true);
            migrationBuilder.AlterColumn<int>(name: "HasReceipt", table: "Transactions", type: "INTEGER", nullable: false, oldClrType: typeof(bool), oldType: "boolean");
            migrationBuilder.AlterColumn<string>(name: "ExpenseType", table: "Transactions", type: "TEXT", nullable: true, oldClrType: typeof(string), oldType: "text", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "Description", table: "Transactions", type: "TEXT", maxLength: 500, nullable: false, oldClrType: typeof(string), oldType: "character varying(500)", oldMaxLength: 500);
            migrationBuilder.AlterColumn<string>(name: "Date", table: "Transactions", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "CreatedAt", table: "Transactions", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "Category", table: "Transactions", type: "TEXT", maxLength: 100, nullable: false, oldClrType: typeof(string), oldType: "character varying(100)", oldMaxLength: 100);
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Transactions", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "UserId", table: "Receipts", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "UpdatedAt", table: "Receipts", type: "TEXT", nullable: true, oldClrType: typeof(DateTime), oldType: "timestamp with time zone", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "TransactionId", table: "Receipts", type: "TEXT", nullable: true, oldClrType: typeof(Guid), oldType: "uuid", oldNullable: true);
            migrationBuilder.AlterColumn<int>(name: "OcrProcessed", table: "Receipts", type: "INTEGER", nullable: false, oldClrType: typeof(bool), oldType: "boolean");
            migrationBuilder.AlterColumn<string>(name: "OcrDataJson", table: "Receipts", type: "TEXT", nullable: true, oldClrType: typeof(string), oldType: "jsonb", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "MimeType", table: "Receipts", type: "TEXT", maxLength: 100, nullable: false, oldClrType: typeof(string), oldType: "character varying(100)", oldMaxLength: 100);
            migrationBuilder.AlterColumn<string>(name: "FileUrl", table: "Receipts", type: "TEXT", maxLength: 500, nullable: false, oldClrType: typeof(string), oldType: "character varying(500)", oldMaxLength: 500);
            migrationBuilder.AlterColumn<int>(name: "FileSize", table: "Receipts", type: "INTEGER", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<string>(name: "FileName", table: "Receipts", type: "TEXT", maxLength: 255, nullable: false, oldClrType: typeof(string), oldType: "character varying(255)", oldMaxLength: 255);
            migrationBuilder.AlterColumn<string>(name: "CreatedAt", table: "Receipts", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Receipts", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "TargetDate", table: "Goals", type: "TEXT", nullable: true, oldClrType: typeof(DateTime), oldType: "timestamp with time zone", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "TargetAmount", table: "Goals", type: "TEXT", nullable: false, oldClrType: typeof(decimal), oldType: "numeric(18,2)");
            migrationBuilder.AlterColumn<string>(name: "Description", table: "Goals", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "character varying(500)", oldMaxLength: 500);
            migrationBuilder.AlterColumn<string>(name: "CurrentAmount", table: "Goals", type: "TEXT", nullable: false, oldClrType: typeof(decimal), oldType: "numeric(18,2)");
            migrationBuilder.AlterColumn<string>(name: "CreatedAt", table: "Goals", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Goals", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "UserId", table: "Expenses", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "text");
            migrationBuilder.AlterColumn<string>(name: "UpdatedAt", table: "Expenses", type: "TEXT", nullable: true, oldClrType: typeof(DateTime), oldType: "timestamp with time zone", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "Name", table: "Expenses", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "character varying(200)", oldMaxLength: 200);
            migrationBuilder.AlterColumn<string>(name: "DueDate", table: "Expenses", type: "TEXT", nullable: true, oldClrType: typeof(DateTime), oldType: "timestamp with time zone", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "Date", table: "Expenses", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "CreatedAt", table: "Expenses", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "Category", table: "Expenses", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "character varying(100)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Expenses", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "UserId", table: "Deposits", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "Source", table: "Deposits", type: "TEXT", nullable: false, oldClrType: typeof(string), oldType: "character varying(200)", oldMaxLength: 200);
            migrationBuilder.AlterColumn<string>(name: "GoalId", table: "Deposits", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");
            migrationBuilder.AlterColumn<string>(name: "Date", table: "Deposits", type: "TEXT", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone");
            migrationBuilder.AlterColumn<string>(name: "Amount", table: "Deposits", type: "TEXT", nullable: false, oldClrType: typeof(decimal), oldType: "numeric(18,2)");
            migrationBuilder.AlterColumn<string>(name: "Id", table: "Deposits", type: "TEXT", nullable: false, oldClrType: typeof(Guid), oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts",
                column: "TransactionId",
                unique: true,
                filter: "\"TransactionId\" IS NOT NULL");
        }
    }
}