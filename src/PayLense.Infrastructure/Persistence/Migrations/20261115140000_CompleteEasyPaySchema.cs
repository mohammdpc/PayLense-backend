using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayLense.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteEasyPaySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Update AspNetUsers (Users) columns
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletionReason",
                table: "AspNetUsers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            // 2. Create FinancialAccounts
            migrationBuilder.CreateTable(
                name: "FinancialAccounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    AccountStatus = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAccounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_FinancialAccounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 3. Update BankCards with EasyPay schema columns
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "BankCards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardToken",
                table: "BankCards",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardFingerprint",
                table: "BankCards",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardholderName",
                table: "BankCards",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BankCards",
                type: "char(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EGP");

            migrationBuilder.AddColumn<string>(
                name: "CardStatus",
                table: "BankCards",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BankCards",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BankCards_ExpiryMonth",
                table: "BankCards",
                sql: "[ExpiryMonth] BETWEEN 1 AND 12");

            migrationBuilder.CreateIndex(
                name: "IX_BankCards_AccountId",
                table: "BankCards",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankCards_FinancialAccounts_AccountId",
                table: "BankCards",
                column: "AccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            // 4. UserContactMethods
            migrationBuilder.CreateTable(
                name: "UserContactMethods",
                columns: table => new
                {
                    ContactId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactType = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    ContactValue = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContactMethods", x => x.ContactId);
                    table.CheckConstraint("CK_UserContactMethods_ContactType", "[ContactType] IN ('Phone', 'Email')");
                    table.ForeignKey(
                        name: "FK_UserContactMethods_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 5. AuthenticationChallenges
            migrationBuilder.CreateTable(
                name: "AuthenticationChallenges",
                columns: table => new
                {
                    ChallengeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChallengeType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    CodeHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttemptsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationChallenges", x => x.ChallengeId);
                    table.ForeignKey(
                        name: "FK_AuthenticationChallenges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 6. ContactVerificationChallenges
            migrationBuilder.CreateTable(
                name: "ContactVerificationChallenges",
                columns: table => new
                {
                    VerificationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    CodeHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttemptsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactVerificationChallenges", x => x.VerificationId);
                    table.ForeignKey(
                        name: "FK_ContactVerificationChallenges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactVerificationChallenges_UserContactMethods_ContactId",
                        column: x => x.ContactId,
                        principalTable: "UserContactMethods",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 7. LoginSessions
            migrationBuilder.CreateTable(
                name: "LoginSessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SessionTokenHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TwoFaVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_LoginSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 8. PhoneChangeRequests
            migrationBuilder.CreateTable(
                name: "PhoneChangeRequests",
                columns: table => new
                {
                    RequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OldContactId = table.Column<long>(type: "bigint", nullable: false),
                    NewPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneChangeRequests", x => x.RequestId);
                    table.CheckConstraint("CK_PhoneChangeRequests_Status", "[Status] IN ('Pending', 'Completed', 'Rejected', 'Expired')");
                    table.ForeignKey(
                        name: "FK_PhoneChangeRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhoneChangeRequests_UserContactMethods_OldContactId",
                        column: x => x.OldContactId,
                        principalTable: "UserContactMethods",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 9. EmailChangeRequests
            migrationBuilder.CreateTable(
                name: "EmailChangeRequests",
                columns: table => new
                {
                    RequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OldContactId = table.Column<long>(type: "bigint", nullable: false),
                    NewEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailChangeRequests", x => x.RequestId);
                    table.CheckConstraint("CK_EmailChangeRequests_Status", "[Status] IN ('Pending', 'Completed', 'Rejected', 'Expired')");
                    table.ForeignKey(
                        name: "FK_EmailChangeRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailChangeRequests_UserContactMethods_OldContactId",
                        column: x => x.OldContactId,
                        principalTable: "UserContactMethods",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 10. AppSettings
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Language = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    Theme = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "System"),
                    NotificationsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_AppSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 11. AuditLogs
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ActionType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // 12. Categories
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IconName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            // 13. Transactions
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalCardToken = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    ExternalCardLast4 = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    SendType = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ExceedsLimit = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LimitOverrideConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Requires2Fa = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TwoFaVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdempotencyKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.CheckConstraint("CK_Transactions_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Transactions_SendType", "[SendType] IN ('Instant', 'Delayed', 'Secure', 'Recurring')");
                    table.CheckConstraint("CK_Transactions_Source", "[Source] IN ('Direct', 'Request', 'Bill', 'Automated')");
                    table.CheckConstraint("CK_Transactions_Status", "[Status] IN ('Pending', 'Processing', 'Completed', 'Failed', 'Cancelled', 'Reversed')");
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_BankCards_ReceiverCardId",
                        column: x => x.ReceiverCardId,
                        principalTable: "BankCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_BankCards_SenderCardId",
                        column: x => x.SenderCardId,
                        principalTable: "BankCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                });

            // 14. LedgerEntries
            migrationBuilder.CreateTable(
                name: "LedgerEntries",
                columns: table => new
                {
                    EntryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryType = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEntries", x => x.EntryId);
                    table.CheckConstraint("CK_LedgerEntries_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_LedgerEntries_EntryType", "[EntryType] IN ('Debit', 'Credit')");
                    table.ForeignKey(
                        name: "FK_LedgerEntries_FinancialAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "FinancialAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerEntries_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 15. TransactionStatusHistory
            migrationBuilder.CreateTable(
                name: "TransactionStatusHistory",
                columns: table => new
                {
                    HistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldStatus = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    NewStatus = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatusHistory", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_TransactionStatusHistory_AspNetUsers_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionStatusHistory_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 16. TransactionOtps
            migrationBuilder.CreateTable(
                name: "TransactionOtps",
                columns: table => new
                {
                    OtpId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodeHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttemptsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionOtps", x => x.OtpId);
                    table.ForeignKey(
                        name: "FK_TransactionOtps_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 17. DelayedSends
            migrationBuilder.CreateTable(
                name: "DelayedSends",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiverNotifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelayedSends", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_DelayedSends_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 18. MoneyRequests
            migrationBuilder.CreateTable(
                name: "MoneyRequests",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RequesterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PayerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequesterCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyRequests", x => x.RequestId);
                    table.CheckConstraint("CK_MoneyRequests_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_MoneyRequests_Status", "[Status] IN ('Pending', 'Accepted', 'Rejected', 'Cancelled', 'Expired')");
                    table.ForeignKey(
                        name: "FK_MoneyRequests_AspNetUsers_PayerId",
                        column: x => x.PayerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MoneyRequests_AspNetUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MoneyRequests_BankCards_RequesterCardId",
                        column: x => x.RequesterCardId,
                        principalTable: "BankCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MoneyRequests_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            // 19. SpendingLimits
            migrationBuilder.CreateTable(
                name: "SpendingLimits",
                columns: table => new
                {
                    LimitId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LimitAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    PeriodType = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpendingLimits", x => x.LimitId);
                    table.CheckConstraint("CK_SpendingLimits_LimitAmount", "[LimitAmount] > 0");
                    table.CheckConstraint("CK_SpendingLimits_PeriodType", "[PeriodType] IN ('Daily', 'Weekly', 'Monthly')");
                    table.ForeignKey(
                        name: "FK_SpendingLimits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpendingLimits_FinancialAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "FinancialAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            // 20. AutomatedSends
            migrationBuilder.CreateTable(
                name: "AutomatedSends",
                columns: table => new
                {
                    AutomatedSendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalCardToken = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    ExternalCardLast4 = table.Column<string>(type: "char(4)", maxLength: 4, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    Frequency = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    IntervalCount = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DayOfMonth = table.Column<byte>(type: "tinyint", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NextRunAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomatedSends", x => x.AutomatedSendId);
                    table.CheckConstraint("CK_AutomatedSends_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_AutomatedSends_DayOfMonth", "[DayOfMonth] IS NULL OR ([DayOfMonth] BETWEEN 1 AND 31)");
                    table.CheckConstraint("CK_AutomatedSends_Frequency", "[Frequency] IN ('Daily', 'Weekly', 'Monthly')");
                    table.CheckConstraint("CK_AutomatedSends_IntervalCount", "[IntervalCount] > 0");
                    table.ForeignKey(
                        name: "FK_AutomatedSends_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomatedSends_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomatedSends_BankCards_CardId",
                        column: x => x.CardId,
                        principalTable: "BankCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomatedSends_BankCards_ReceiverCardId",
                        column: x => x.ReceiverCardId,
                        principalTable: "BankCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutomatedSends_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                });

            // 21. AutomatedSendRuns
            migrationBuilder.CreateTable(
                name: "AutomatedSendRuns",
                columns: table => new
                {
                    RunId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AutomatedSendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomatedSendRuns", x => x.RunId);
                    table.ForeignKey(
                        name: "FK_AutomatedSendRuns_AutomatedSends_AutomatedSendId",
                        column: x => x.AutomatedSendId,
                        principalTable: "AutomatedSends",
                        principalColumn: "AutomatedSendId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutomatedSendRuns_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            // 22. Bills
            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    BillName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    PublicTokenHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.BillId);
                    table.CheckConstraint("CK_Bills_TargetAmount", "[TargetAmount] > 0");
                    table.ForeignKey(
                        name: "FK_Bills_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bills_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bills_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                });

            // 23. BillParticipants
            migrationBuilder.CreateTable(
                name: "BillParticipants",
                columns: table => new
                {
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillParticipants", x => new { x.BillId, x.UserId });
                    table.CheckConstraint("CK_BillParticipants_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_BillParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillParticipants_BankCards_CardId",
                        column: x => x.CardId,
                        principalTable: "BankCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillParticipants_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "BillId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillParticipants_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            // 24. BillSettlements
            migrationBuilder.CreateTable(
                name: "BillSettlements",
                columns: table => new
                {
                    SettlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillSettlements", x => x.SettlementId);
                    table.ForeignKey(
                        name: "FK_BillSettlements_AspNetUsers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillSettlements_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "BillId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 25. Receipts
            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    ReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: true),
                    ReceiptDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SuggestedCategoryId = table.Column<int>(type: "int", nullable: true),
                    AiConfidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    IncludeInReport = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.ReceiptId);
                    table.CheckConstraint("CK_Receipts_AiConfidence", "[AiConfidence] IS NULL OR ([AiConfidence] BETWEEN 0 AND 1)");
                    table.ForeignKey(
                        name: "FK_Receipts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Receipts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receipts_Categories_SuggestedCategoryId",
                        column: x => x.SuggestedCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receipts_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            // 26. ReportSnapshots
            migrationBuilder.CreateTable(
                name: "ReportSnapshots",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    ReportData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSnapshots", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_ReportSnapshots_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 27. Notifications
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NotificationType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    RelatedTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Transactions_RelatedTransactionId",
                        column: x => x.RelatedTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            // 28. NotificationPreferences
            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SecurityNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MarketingNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PushNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 29. ChatSessions
            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    ChatSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SessionStatus = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.ChatSessionId);
                    table.ForeignKey(
                        name: "FK_ChatSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 30. QA_Items
            migrationBuilder.CreateTable(
                name: "QA_Items",
                columns: table => new
                {
                    QaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Topic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QA_Items", x => x.QaId);
                });

            // 31. QA_Translations
            migrationBuilder.CreateTable(
                name: "QA_Translations",
                columns: table => new
                {
                    QaId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Question = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QA_Translations", x => new { x.QaId, x.Language });
                    table.ForeignKey(
                        name: "FK_QA_Translations_QA_Items_QaId",
                        column: x => x.QaId,
                        principalTable: "QA_Items",
                        principalColumn: "QaId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 32. ChatMessages
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    MessageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderType = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    MatchedQaId = table.Column<int>(type: "int", nullable: true),
                    ConfidenceScore = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    IsFallback = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsHelpful = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.MessageId);
                    table.CheckConstraint("CK_ChatMessages_ConfidenceScore", "[ConfidenceScore] IS NULL OR ([ConfidenceScore] BETWEEN 0 AND 1)");
                    table.CheckConstraint("CK_ChatMessages_SenderType", "[SenderType] IN ('User', 'Bot')");
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "ChatSessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_QA_Items_MatchedQaId",
                        column: x => x.MatchedQaId,
                        principalTable: "QA_Items",
                        principalColumn: "QaId",
                        onDelete: ReferentialAction.SetNull);
                });

            // Indexes for all new tables
            migrationBuilder.CreateIndex(name: "IX_FinancialAccounts_UserId", table: "FinancialAccounts", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_UserContactMethods_UserId", table: "UserContactMethods", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_AuthenticationChallenges_UserId", table: "AuthenticationChallenges", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_ContactVerificationChallenges_UserId", table: "ContactVerificationChallenges", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_ContactVerificationChallenges_ContactId", table: "ContactVerificationChallenges", column: "ContactId");
            migrationBuilder.CreateIndex(name: "IX_LoginSessions_UserId", table: "LoginSessions", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_PhoneChangeRequests_UserId", table: "PhoneChangeRequests", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_PhoneChangeRequests_OldContactId", table: "PhoneChangeRequests", column: "OldContactId");
            migrationBuilder.CreateIndex(name: "IX_EmailChangeRequests_UserId", table: "EmailChangeRequests", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_EmailChangeRequests_OldContactId", table: "EmailChangeRequests", column: "OldContactId");
            migrationBuilder.CreateIndex(name: "IX_AuditLogs_UserId", table: "AuditLogs", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Categories_CategoryName", table: "Categories", column: "CategoryName", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Transactions_SenderId_IdempotencyKey", table: "Transactions", columns: new[] { "SenderId", "IdempotencyKey" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_Transactions_ReceiverId", table: "Transactions", column: "ReceiverId");
            migrationBuilder.CreateIndex(name: "IX_Transactions_SenderCardId", table: "Transactions", column: "SenderCardId");
            migrationBuilder.CreateIndex(name: "IX_Transactions_ReceiverCardId", table: "Transactions", column: "ReceiverCardId");
            migrationBuilder.CreateIndex(name: "IX_Transactions_CategoryId", table: "Transactions", column: "CategoryId");
            migrationBuilder.CreateIndex(name: "IX_LedgerEntries_TransactionId", table: "LedgerEntries", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_LedgerEntries_AccountId", table: "LedgerEntries", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_TransactionStatusHistory_TransactionId", table: "TransactionStatusHistory", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_TransactionStatusHistory_ChangedBy", table: "TransactionStatusHistory", column: "ChangedBy");
            migrationBuilder.CreateIndex(name: "IX_TransactionOtps_TransactionId", table: "TransactionOtps", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_MoneyRequests_RequesterId", table: "MoneyRequests", column: "RequesterId");
            migrationBuilder.CreateIndex(name: "IX_MoneyRequests_PayerId", table: "MoneyRequests", column: "PayerId");
            migrationBuilder.CreateIndex(name: "IX_MoneyRequests_RequesterCardId", table: "MoneyRequests", column: "RequesterCardId");
            migrationBuilder.CreateIndex(name: "IX_MoneyRequests_TransactionId", table: "MoneyRequests", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_SpendingLimits_UserId", table: "SpendingLimits", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_SpendingLimits_AccountId", table: "SpendingLimits", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_AutomatedSends_UserId", table: "AutomatedSends", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_AutomatedSends_ReceiverId", table: "AutomatedSends", column: "ReceiverId");
            migrationBuilder.CreateIndex(name: "IX_AutomatedSends_CardId", table: "AutomatedSends", column: "CardId");
            migrationBuilder.CreateIndex(name: "IX_AutomatedSends_ReceiverCardId", table: "AutomatedSends", column: "ReceiverCardId");
            migrationBuilder.CreateIndex(name: "IX_AutomatedSends_CategoryId", table: "AutomatedSends", column: "CategoryId");
            migrationBuilder.CreateIndex(name: "IX_AutomatedSendRuns_AutomatedSendId_ScheduledFor", table: "AutomatedSendRuns", columns: new[] { "AutomatedSendId", "ScheduledFor" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_AutomatedSendRuns_TransactionId", table: "AutomatedSendRuns", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_Bills_PublicTokenHash", table: "Bills", column: "PublicTokenHash", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Bills_CreatorId", table: "Bills", column: "CreatorId");
            migrationBuilder.CreateIndex(name: "IX_Bills_ReceiverId", table: "Bills", column: "ReceiverId");
            migrationBuilder.CreateIndex(name: "IX_Bills_CategoryId", table: "Bills", column: "CategoryId");
            migrationBuilder.CreateIndex(name: "IX_BillParticipants_UserId", table: "BillParticipants", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_BillParticipants_CardId", table: "BillParticipants", column: "CardId");
            migrationBuilder.CreateIndex(name: "IX_BillParticipants_TransactionId", table: "BillParticipants", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_BillSettlements_BillId", table: "BillSettlements", column: "BillId");
            migrationBuilder.CreateIndex(name: "IX_BillSettlements_ApprovedBy", table: "BillSettlements", column: "ApprovedBy");
            migrationBuilder.CreateIndex(name: "IX_Receipts_UserId", table: "Receipts", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Receipts_TransactionId", table: "Receipts", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_Receipts_CategoryId", table: "Receipts", column: "CategoryId");
            migrationBuilder.CreateIndex(name: "IX_Receipts_SuggestedCategoryId", table: "Receipts", column: "SuggestedCategoryId");
            migrationBuilder.CreateIndex(name: "IX_ReportSnapshots_UserId", table: "ReportSnapshots", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Notifications_UserId", table: "Notifications", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Notifications_RelatedTransactionId", table: "Notifications", column: "RelatedTransactionId");
            migrationBuilder.CreateIndex(name: "IX_ChatSessions_UserId", table: "ChatSessions", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_ChatMessages_ChatSessionId", table: "ChatMessages", column: "ChatSessionId");
            migrationBuilder.CreateIndex(name: "IX_ChatMessages_MatchedQaId", table: "ChatMessages", column: "MatchedQaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AppSettings");
            migrationBuilder.DropTable(name: "AuditLogs");
            migrationBuilder.DropTable(name: "AuthenticationChallenges");
            migrationBuilder.DropTable(name: "AutomatedSendRuns");
            migrationBuilder.DropTable(name: "BillParticipants");
            migrationBuilder.DropTable(name: "BillSettlements");
            migrationBuilder.DropTable(name: "ChatMessages");
            migrationBuilder.DropTable(name: "ContactVerificationChallenges");
            migrationBuilder.DropTable(name: "DelayedSends");
            migrationBuilder.DropTable(name: "EmailChangeRequests");
            migrationBuilder.DropTable(name: "LedgerEntries");
            migrationBuilder.DropTable(name: "LoginSessions");
            migrationBuilder.DropTable(name: "MoneyRequests");
            migrationBuilder.DropTable(name: "NotificationPreferences");
            migrationBuilder.DropTable(name: "Notifications");
            migrationBuilder.DropTable(name: "PhoneChangeRequests");
            migrationBuilder.DropTable(name: "QA_Translations");
            migrationBuilder.DropTable(name: "Receipts");
            migrationBuilder.DropTable(name: "ReportSnapshots");
            migrationBuilder.DropTable(name: "SpendingLimits");
            migrationBuilder.DropTable(name: "TransactionOtps");
            migrationBuilder.DropTable(name: "TransactionStatusHistory");
            migrationBuilder.DropTable(name: "AutomatedSends");
            migrationBuilder.DropTable(name: "Bills");
            migrationBuilder.DropTable(name: "ChatSessions");
            migrationBuilder.DropTable(name: "UserContactMethods");
            migrationBuilder.DropTable(name: "QA_Items");
            migrationBuilder.DropTable(name: "Transactions");
            migrationBuilder.DropTable(name: "Categories");

            migrationBuilder.DropForeignKey(name: "FK_BankCards_FinancialAccounts_AccountId", table: "BankCards");
            migrationBuilder.DropIndex(name: "IX_BankCards_AccountId", table: "BankCards");
            migrationBuilder.DropCheckConstraint(name: "CK_BankCards_ExpiryMonth", table: "BankCards");

            migrationBuilder.DropTable(name: "FinancialAccounts");

            migrationBuilder.DropColumn(name: "AccountId", table: "BankCards");
            migrationBuilder.DropColumn(name: "CardToken", table: "BankCards");
            migrationBuilder.DropColumn(name: "CardFingerprint", table: "BankCards");
            migrationBuilder.DropColumn(name: "CardholderName", table: "BankCards");
            migrationBuilder.DropColumn(name: "Currency", table: "BankCards");
            migrationBuilder.DropColumn(name: "CardStatus", table: "BankCards");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "BankCards");

            migrationBuilder.DropColumn(name: "IsActive", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "DeletedAt", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "DeletionReason", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "AspNetUsers");
        }
    }
}
