using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayLense.Domain.Entities;
using PayLense.Infrastructure.Identity;

namespace PayLense.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // 1. Auth, Contacts, Sessions & Settings Tables
    public DbSet<UserContactMethod> UserContactMethods => Set<UserContactMethod>();
    public DbSet<AuthenticationChallenge> AuthenticationChallenges => Set<AuthenticationChallenge>();
    public DbSet<ContactVerificationChallenge> ContactVerificationChallenges => Set<ContactVerificationChallenge>();
    public DbSet<LoginSession> LoginSessions => Set<LoginSession>();
    public DbSet<PhoneChangeRequest> PhoneChangeRequests => Set<PhoneChangeRequest>();
    public DbSet<EmailChangeRequest> EmailChangeRequests => Set<EmailChangeRequest>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TrustedDevice> TrustedDevices => Set<TrustedDevice>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    // 2. Financial Accounts, Bank Cards & Spending Limits
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();
    public DbSet<BankCard> BankCards => Set<BankCard>();
    public DbSet<SpendingLimit> SpendingLimits => Set<SpendingLimit>();

    // 3. Categories, Transactions, Ledger & Delayed/Secure Transfers
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<TransactionStatusHistory> TransactionStatusHistories => Set<TransactionStatusHistory>();
    public DbSet<TransactionOtp> TransactionOtps => Set<TransactionOtp>();
    public DbSet<DelayedSend> DelayedSends => Set<DelayedSend>();

    // 4. Money Requests & Automated Recurring Transfers
    public DbSet<MoneyRequest> MoneyRequests => Set<MoneyRequest>();
    public DbSet<AutomatedSend> AutomatedSends => Set<AutomatedSend>();
    public DbSet<AutomatedSendRun> AutomatedSendRuns => Set<AutomatedSendRun>();

    // 5. Bill Splitting & QR Bills
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<BillParticipant> BillParticipants => Set<BillParticipant>();
    public DbSet<BillSettlement> BillSettlements => Set<BillSettlement>();

    // 6. Receipts & Report Snapshots
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReportSnapshot> ReportSnapshots => Set<ReportSnapshot>();

    // 7. Notifications & Preferences
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    // 8. Support Chatbot & Multi-language FAQ (QA)
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<QaItem> QaItems => Set<QaItem>();
    public DbSet<QaTranslation> QaTranslations => Set<QaTranslation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --------------------------------------------------------------------
        // 1. ApplicationUser (Users)
        // --------------------------------------------------------------------
        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.Property(x => x.DeletionReason).HasMaxLength(250);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // --------------------------------------------------------------------
        // Existing Auth Tables (RefreshTokens, TrustedDevices, OtpCodes)
        // --------------------------------------------------------------------
        builder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            e.Property(x => x.ReplacedByTokenHash).HasMaxLength(128);
            e.Property(x => x.DeviceId).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TrustedDevice>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.DeviceIdentifier).HasMaxLength(100).IsRequired();
            e.Property(x => x.DeviceName).HasMaxLength(200);
            e.HasIndex(x => new { x.UserId, x.DeviceIdentifier }).IsUnique();
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OtpCode>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.CodeHash).HasMaxLength(128).IsRequired();
            e.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(x => new { x.UserId, x.Purpose });
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 2. UserContactMethods
        // --------------------------------------------------------------------
        builder.Entity<UserContactMethod>(e =>
        {
            e.ToTable("UserContactMethods", t =>
                t.HasCheckConstraint("CK_UserContactMethods_ContactType", "[ContactType] IN ('Phone', 'Email')"));
            e.HasKey(x => x.ContactId);
            e.Property(x => x.ContactId).UseIdentityColumn();
            e.Property(x => x.ContactType).HasColumnType("varchar(10)").HasMaxLength(10).IsRequired();
            e.Property(x => x.ContactValue).HasMaxLength(254).IsRequired();
            e.Property(x => x.IsPrimary).HasDefaultValue(false);
            e.Property(x => x.IsVerified).HasDefaultValue(false);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 3. AuthenticationChallenges
        // --------------------------------------------------------------------
        builder.Entity<AuthenticationChallenge>(e =>
        {
            e.ToTable("AuthenticationChallenges");
            e.HasKey(x => x.ChallengeId);
            e.Property(x => x.ChallengeId).UseIdentityColumn();
            e.Property(x => x.ChallengeType).HasColumnType("varchar(30)").HasMaxLength(30).IsRequired();
            e.Property(x => x.CodeHash).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
            e.Property(x => x.AttemptsCount).HasDefaultValue(0);
            e.Property(x => x.IsUsed).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 4. ContactVerificationChallenges
        // --------------------------------------------------------------------
        builder.Entity<ContactVerificationChallenge>(e =>
        {
            e.ToTable("ContactVerificationChallenges");
            e.HasKey(x => x.VerificationId);
            e.Property(x => x.VerificationId).UseIdentityColumn();
            e.Property(x => x.CodeHash).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
            e.Property(x => x.AttemptsCount).HasDefaultValue(0);
            e.Property(x => x.IsVerified).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.ContactId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Contact).WithMany(c => c.VerificationChallenges)
                .HasForeignKey(x => x.ContactId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 5. LoginSessions
        // --------------------------------------------------------------------
        builder.Entity<LoginSession>(e =>
        {
            e.ToTable("LoginSessions");
            e.HasKey(x => x.SessionId);
            e.Property(x => x.SessionId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.SessionTokenHash).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
            e.Property(x => x.IpAddress).HasColumnType("varchar(45)").HasMaxLength(45);
            e.Property(x => x.UserAgent).HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 6. PhoneChangeRequests
        // --------------------------------------------------------------------
        builder.Entity<PhoneChangeRequest>(e =>
        {
            e.ToTable("PhoneChangeRequests", t =>
                t.HasCheckConstraint("CK_PhoneChangeRequests_Status", "[Status] IN ('Pending', 'Completed', 'Rejected', 'Expired')"));
            e.HasKey(x => x.RequestId);
            e.Property(x => x.RequestId).UseIdentityColumn();
            e.Property(x => x.NewPhoneNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.RequestedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.OldContactId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.OldContact).WithMany(c => c.PhoneChangeRequests)
                .HasForeignKey(x => x.OldContactId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 7. EmailChangeRequests
        // --------------------------------------------------------------------
        builder.Entity<EmailChangeRequest>(e =>
        {
            e.ToTable("EmailChangeRequests", t =>
                t.HasCheckConstraint("CK_EmailChangeRequests_Status", "[Status] IN ('Pending', 'Completed', 'Rejected', 'Expired')"));
            e.HasKey(x => x.RequestId);
            e.Property(x => x.RequestId).UseIdentityColumn();
            e.Property(x => x.NewEmail).HasMaxLength(254).IsRequired();
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.RequestedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.OldContactId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.OldContact).WithMany(c => c.EmailChangeRequests)
                .HasForeignKey(x => x.OldContactId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 8. AppSettings
        // --------------------------------------------------------------------
        builder.Entity<AppSetting>(e =>
        {
            e.ToTable("AppSettings");
            e.HasKey(x => x.UserId);
            e.Property(x => x.Language).HasColumnType("varchar(10)").HasMaxLength(10).HasDefaultValue("en").IsRequired();
            e.Property(x => x.Theme).HasColumnType("varchar(10)").HasMaxLength(10).HasDefaultValue("System").IsRequired();
            e.Property(x => x.NotificationsEnabled).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasOne<ApplicationUser>().WithOne()
                .HasForeignKey<AppSetting>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 9. AuditLogs
        // --------------------------------------------------------------------
        builder.Entity<AuditLog>(e =>
        {
            e.ToTable("AuditLogs");
            e.HasKey(x => x.LogId);
            e.Property(x => x.LogId).UseIdentityColumn();
            e.Property(x => x.ActionType).HasColumnType("varchar(50)").HasMaxLength(50).IsRequired();
            e.Property(x => x.EntityName).HasColumnType("varchar(100)").HasMaxLength(100);
            e.Property(x => x.EntityId).HasMaxLength(100);
            e.Property(x => x.IpAddress).HasColumnType("varchar(45)").HasMaxLength(45);
            e.Property(x => x.Details).HasMaxLength(1000);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        // --------------------------------------------------------------------
        // 10. FinancialAccounts
        // --------------------------------------------------------------------
        builder.Entity<FinancialAccount>(e =>
        {
            e.ToTable("FinancialAccounts");
            e.HasKey(x => x.AccountId);
            e.Property(x => x.AccountId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.AccountName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.AccountStatus).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 11. BankCards
        // --------------------------------------------------------------------
        builder.Entity<BankCard>(e =>
        {
            e.ToTable("BankCards", t =>
                t.HasCheckConstraint("CK_BankCards_ExpiryMonth", "[ExpiryMonth] BETWEEN 1 AND 12"));
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.CardNumberEncrypted).IsRequired();
            e.Property(x => x.CardToken).HasColumnType("varchar(255)").HasMaxLength(255);
            e.Property(x => x.CardFingerprint).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
            e.Property(x => x.Last4Digits).HasColumnType("char(4)").HasMaxLength(4).IsRequired();
            e.Property(x => x.CardholderName).HasMaxLength(100);
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).HasDefaultValue("EGP").IsRequired();
            e.Property(x => x.CardStatus).HasColumnType("varchar(20)").HasMaxLength(20).HasDefaultValue("Active").IsRequired();
            e.Property(x => x.IsDefault).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.AccountId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Account).WithMany(a => a.BankCards)
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 12. Categories
        // --------------------------------------------------------------------
        builder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.HasKey(x => x.CategoryId);
            e.Property(x => x.CategoryId).UseIdentityColumn();
            e.Property(x => x.CategoryName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(250);
            e.Property(x => x.IconName).HasColumnType("varchar(50)").HasMaxLength(50);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => x.CategoryName).IsUnique();
        });

        // --------------------------------------------------------------------
        // 13. Transactions
        // --------------------------------------------------------------------
        builder.Entity<Transaction>(e =>
        {
            e.ToTable("Transactions", t =>
            {
                t.HasCheckConstraint("CK_Transactions_Amount", "[Amount] > 0");
                t.HasCheckConstraint("CK_Transactions_SendType", "[SendType] IN ('Instant', 'Delayed', 'Secure', 'Recurring')");
                t.HasCheckConstraint("CK_Transactions_Source", "[Source] IN ('Direct', 'Request', 'Bill', 'Automated')");
                t.HasCheckConstraint("CK_Transactions_Status", "[Status] IN ('Pending', 'Processing', 'Completed', 'Failed', 'Cancelled', 'Reversed')");
            });
            e.HasKey(x => x.TransactionId);
            e.Property(x => x.TransactionId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.ExternalCardToken).HasColumnType("varchar(255)").HasMaxLength(255);
            e.Property(x => x.ExternalCardLast4).HasColumnType("char(4)").HasMaxLength(4);
            e.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.SendType).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.Source).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.ExceedsLimit).HasDefaultValue(false);
            e.Property(x => x.Requires2Fa).HasDefaultValue(false);
            e.Property(x => x.FailureReason).HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.RowVersion).IsRowVersion();

            e.HasIndex(x => new { x.SenderId, x.IdempotencyKey }).IsUnique();
            e.HasIndex(x => x.ReceiverId);
            e.HasIndex(x => x.SenderCardId);
            e.HasIndex(x => x.ReceiverCardId);
            e.HasIndex(x => x.CategoryId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SenderCard).WithMany(c => c.SentTransactions)
                .HasForeignKey(x => x.SenderCardId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ReceiverCard).WithMany(c => c.ReceivedTransactions)
                .HasForeignKey(x => x.ReceiverCardId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Category).WithMany(c => c.Transactions)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        // --------------------------------------------------------------------
        // 14. LedgerEntries
        // --------------------------------------------------------------------
        builder.Entity<LedgerEntry>(e =>
        {
            e.ToTable("LedgerEntries", t =>
            {
                t.HasCheckConstraint("CK_LedgerEntries_EntryType", "[EntryType] IN ('Debit', 'Credit')");
                t.HasCheckConstraint("CK_LedgerEntries_Amount", "[Amount] > 0");
            });
            e.HasKey(x => x.EntryId);
            e.Property(x => x.EntryId).UseIdentityColumn();
            e.Property(x => x.EntryType).HasColumnType("varchar(10)").HasMaxLength(10).IsRequired();
            e.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.TransactionId);
            e.HasIndex(x => x.AccountId);

            e.HasOne(x => x.Transaction).WithMany(t => t.LedgerEntries)
                .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Account).WithMany(a => a.LedgerEntries)
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 15. TransactionStatusHistory
        // --------------------------------------------------------------------
        builder.Entity<TransactionStatusHistory>(e =>
        {
            e.ToTable("TransactionStatusHistory");
            e.HasKey(x => x.HistoryId);
            e.Property(x => x.HistoryId).UseIdentityColumn();
            e.Property(x => x.OldStatus).HasColumnType("varchar(20)").HasMaxLength(20);
            e.Property(x => x.NewStatus).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.ChangeReason).HasMaxLength(500);
            e.Property(x => x.ChangedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.TransactionId);
            e.HasIndex(x => x.ChangedBy);

            e.HasOne(x => x.Transaction).WithMany(t => t.StatusHistory)
                .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ChangedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 16. TransactionOtps
        // --------------------------------------------------------------------
        builder.Entity<TransactionOtp>(e =>
        {
            e.ToTable("TransactionOtps");
            e.HasKey(x => x.OtpId);
            e.Property(x => x.OtpId).UseIdentityColumn();
            e.Property(x => x.CodeHash).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
            e.Property(x => x.AttemptsCount).HasDefaultValue(0);
            e.Property(x => x.IsUsed).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.TransactionId);

            e.HasOne(x => x.Transaction).WithMany(t => t.TransactionOtps)
                .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 17. DelayedSends
        // --------------------------------------------------------------------
        builder.Entity<DelayedSend>(e =>
        {
            e.ToTable("DelayedSends");
            e.HasKey(x => x.TransactionId);
            e.Property(x => x.MessageText).HasMaxLength(250);

            e.HasOne(x => x.Transaction).WithOne(t => t.DelayedSend)
                .HasForeignKey<DelayedSend>(x => x.TransactionId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 18. MoneyRequests
        // --------------------------------------------------------------------
        builder.Entity<MoneyRequest>(e =>
        {
            e.ToTable("MoneyRequests", t =>
            {
                t.HasCheckConstraint("CK_MoneyRequests_Amount", "[Amount] > 0");
                t.HasCheckConstraint("CK_MoneyRequests_Status", "[Status] IN ('Pending', 'Accepted', 'Rejected', 'Cancelled', 'Expired')");
            });
            e.HasKey(x => x.RequestId);
            e.Property(x => x.RequestId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.Note).HasMaxLength(250);
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.RequesterId);
            e.HasIndex(x => x.PayerId);
            e.HasIndex(x => x.RequesterCardId);
            e.HasIndex(x => x.TransactionId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.RequesterId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.PayerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.RequesterCard).WithMany(c => c.MoneyRequests)
                .HasForeignKey(x => x.RequesterCardId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Transaction).WithMany(t => t.MoneyRequests)
                .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 19. SpendingLimits
        // --------------------------------------------------------------------
        builder.Entity<SpendingLimit>(e =>
        {
            e.ToTable("SpendingLimits", t =>
            {
                t.HasCheckConstraint("CK_SpendingLimits_LimitAmount", "[LimitAmount] > 0");
                t.HasCheckConstraint("CK_SpendingLimits_PeriodType", "[PeriodType] IN ('Daily', 'Weekly', 'Monthly')");
            });
            e.HasKey(x => x.LimitId);
            e.Property(x => x.LimitId).UseIdentityColumn();
            e.Property(x => x.LimitAmount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.PeriodType).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.AccountId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Account).WithMany(a => a.SpendingLimits)
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 20. AutomatedSends
        // --------------------------------------------------------------------
        builder.Entity<AutomatedSend>(e =>
        {
            e.ToTable("AutomatedSends", t =>
            {
                t.HasCheckConstraint("CK_AutomatedSends_Amount", "[Amount] > 0");
                t.HasCheckConstraint("CK_AutomatedSends_Frequency", "[Frequency] IN ('Daily', 'Weekly', 'Monthly')");
                t.HasCheckConstraint("CK_AutomatedSends_IntervalCount", "[IntervalCount] > 0");
                t.HasCheckConstraint("CK_AutomatedSends_DayOfMonth", "[DayOfMonth] IS NULL OR ([DayOfMonth] BETWEEN 1 AND 31)");
            });
            e.HasKey(x => x.AutomatedSendId);
            e.Property(x => x.AutomatedSendId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.ExternalCardToken).HasColumnType("varchar(255)").HasMaxLength(255);
            e.Property(x => x.ExternalCardLast4).HasColumnType("char(4)").HasMaxLength(4);
            e.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.Frequency).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.IntervalCount).HasDefaultValue(1);
            e.Property(x => x.StartDate).HasColumnType("date");
            e.Property(x => x.EndDate).HasColumnType("date");
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.ReceiverId);
            e.HasIndex(x => x.CardId);
            e.HasIndex(x => x.ReceiverCardId);
            e.HasIndex(x => x.CategoryId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Card).WithMany(c => c.SourceAutomatedSends)
                .HasForeignKey(x => x.CardId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ReceiverCard).WithMany(c => c.ReceiverAutomatedSends)
                .HasForeignKey(x => x.ReceiverCardId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Category).WithMany(c => c.AutomatedSends)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        // --------------------------------------------------------------------
        // 21. AutomatedSendRuns
        // --------------------------------------------------------------------
        builder.Entity<AutomatedSendRun>(e =>
        {
            e.ToTable("AutomatedSendRuns");
            e.HasKey(x => x.RunId);
            e.Property(x => x.RunId).UseIdentityColumn();
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.FailureReason).HasMaxLength(500);

            e.HasIndex(x => new { x.AutomatedSendId, x.ScheduledFor }).IsUnique();
            e.HasIndex(x => x.TransactionId);

            e.HasOne(x => x.AutomatedSend).WithMany(a => a.Runs)
                .HasForeignKey(x => x.AutomatedSendId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Transaction).WithMany(t => t.AutomatedSendRuns)
                .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 22. Bills
        // --------------------------------------------------------------------
        builder.Entity<Bill>(e =>
        {
            e.ToTable("Bills", t =>
                t.HasCheckConstraint("CK_Bills_TargetAmount", "[TargetAmount] > 0"));
            e.HasKey(x => x.BillId);
            e.Property(x => x.BillId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.BillName).HasMaxLength(150).IsRequired();
            e.Property(x => x.TargetAmount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.PublicTokenHash).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.RowVersion).IsRowVersion();

            e.HasIndex(x => x.PublicTokenHash).IsUnique();
            e.HasIndex(x => x.CreatorId);
            e.HasIndex(x => x.ReceiverId);
            e.HasIndex(x => x.CategoryId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Category).WithMany(c => c.Bills)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        // --------------------------------------------------------------------
        // 23. BillParticipants
        // --------------------------------------------------------------------
        builder.Entity<BillParticipant>(e =>
        {
            e.ToTable("BillParticipants", t =>
                t.HasCheckConstraint("CK_BillParticipants_Amount", "[Amount] > 0"));
            e.HasKey(x => new { x.BillId, x.UserId });
            e.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.JoinedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.CardId);
            e.HasIndex(x => x.TransactionId);

            e.HasOne(x => x.Bill).WithMany(b => b.Participants)
                .HasForeignKey(x => x.BillId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Card).WithMany(c => c.BillParticipants)
                .HasForeignKey(x => x.CardId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Transaction).WithMany(t => t.BillParticipants)
                .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 24. BillSettlements
        // --------------------------------------------------------------------
        builder.Entity<BillSettlement>(e =>
        {
            e.ToTable("BillSettlements");
            e.HasKey(x => x.SettlementId);
            e.Property(x => x.SettlementId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.Status).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.BillId);
            e.HasIndex(x => x.ApprovedBy);

            e.HasOne(x => x.Bill).WithMany(b => b.Settlements)
                .HasForeignKey(x => x.BillId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 25. Receipts
        // --------------------------------------------------------------------
        builder.Entity<Receipt>(e =>
        {
            e.ToTable("Receipts", t =>
                t.HasCheckConstraint("CK_Receipts_AiConfidence", "[AiConfidence] IS NULL OR ([AiConfidence] BETWEEN 0 AND 1)"));
            e.HasKey(x => x.ReceiptId);
            e.Property(x => x.ReceiptId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.ImagePath).HasMaxLength(500).IsRequired();
            e.Property(x => x.MerchantName).HasMaxLength(200);
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3);
            e.Property(x => x.ReceiptDate).HasColumnType("date");
            e.Property(x => x.AiConfidence).HasPrecision(5, 4);
            e.Property(x => x.IncludeInReport).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.TransactionId);
            e.HasIndex(x => x.CategoryId);
            e.HasIndex(x => x.SuggestedCategoryId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Transaction).WithMany(t => t.Receipts)
                .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Category).WithMany(c => c.Receipts)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SuggestedCategory).WithMany(c => c.SuggestedReceipts)
                .HasForeignKey(x => x.SuggestedCategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 26. ReportSnapshots
        // --------------------------------------------------------------------
        builder.Entity<ReportSnapshot>(e =>
        {
            e.ToTable("ReportSnapshots");
            e.HasKey(x => x.ReportId);
            e.Property(x => x.ReportId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.ReportType).HasColumnType("varchar(30)").HasMaxLength(30).IsRequired();
            e.Property(x => x.PeriodStart).HasColumnType("date");
            e.Property(x => x.PeriodEnd).HasColumnType("date");
            e.Property(x => x.Currency).HasColumnType("char(3)").HasMaxLength(3).IsRequired();
            e.Property(x => x.ReportData).IsRequired();
            e.Property(x => x.GeneratedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.UserId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 27. Notifications
        // --------------------------------------------------------------------
        builder.Entity<Notification>(e =>
        {
            e.ToTable("Notifications");
            e.HasKey(x => x.NotificationId);
            e.Property(x => x.NotificationId).UseIdentityColumn();
            e.Property(x => x.Title).HasMaxLength(150).IsRequired();
            e.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            e.Property(x => x.NotificationType).HasColumnType("varchar(30)").HasMaxLength(30).IsRequired();
            e.Property(x => x.IsRead).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.RelatedTransactionId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.RelatedTransaction).WithMany(t => t.Notifications)
                .HasForeignKey(x => x.RelatedTransactionId).OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------------------------------------------------------
        // 28. NotificationPreferences
        // --------------------------------------------------------------------
        builder.Entity<NotificationPreference>(e =>
        {
            e.ToTable("NotificationPreferences");
            e.HasKey(x => x.UserId);
            e.Property(x => x.TransactionNotifications).HasDefaultValue(true);
            e.Property(x => x.SecurityNotifications).HasDefaultValue(true);
            e.Property(x => x.MarketingNotifications).HasDefaultValue(false);
            e.Property(x => x.EmailNotifications).HasDefaultValue(true);
            e.Property(x => x.PushNotifications).HasDefaultValue(true);
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne<ApplicationUser>().WithOne()
                .HasForeignKey<NotificationPreference>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 29. ChatSessions
        // --------------------------------------------------------------------
        builder.Entity<ChatSession>(e =>
        {
            e.ToTable("ChatSessions");
            e.HasKey(x => x.ChatSessionId);
            e.Property(x => x.ChatSessionId).HasDefaultValueSql("NEWID()");
            e.Property(x => x.SessionStatus).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.StartedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.UserId);

            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------------------------------------------------------
        // 30. ChatMessages
        // --------------------------------------------------------------------
        builder.Entity<ChatMessage>(e =>
        {
            e.ToTable("ChatMessages", t =>
            {
                t.HasCheckConstraint("CK_ChatMessages_SenderType", "[SenderType] IN ('User', 'Bot')");
                t.HasCheckConstraint("CK_ChatMessages_ConfidenceScore", "[ConfidenceScore] IS NULL OR ([ConfidenceScore] BETWEEN 0 AND 1)");
            });
            e.HasKey(x => x.MessageId);
            e.Property(x => x.MessageId).UseIdentityColumn();
            e.Property(x => x.SenderType).HasColumnType("varchar(10)").HasMaxLength(10).IsRequired();
            e.Property(x => x.Content).IsRequired();
            e.Property(x => x.Language).HasColumnType("varchar(10)").HasMaxLength(10);
            e.Property(x => x.ConfidenceScore).HasPrecision(5, 4);
            e.Property(x => x.IsFallback).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => x.ChatSessionId);
            e.HasIndex(x => x.MatchedQaId);

            e.HasOne(x => x.ChatSession).WithMany(s => s.Messages)
                .HasForeignKey(x => x.ChatSessionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.MatchedQa).WithMany(q => q.ChatMessages)
                .HasForeignKey(x => x.MatchedQaId).OnDelete(DeleteBehavior.SetNull);
        });

        // --------------------------------------------------------------------
        // 31. QA_Items
        // --------------------------------------------------------------------
        builder.Entity<QaItem>(e =>
        {
            e.ToTable("QA_Items");
            e.HasKey(x => x.QaId);
            e.Property(x => x.QaId).UseIdentityColumn();
            e.Property(x => x.Topic).HasMaxLength(100).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // --------------------------------------------------------------------
        // 32. QA_Translations
        // --------------------------------------------------------------------
        builder.Entity<QaTranslation>(e =>
        {
            e.ToTable("QA_Translations");
            e.HasKey(x => new { x.QaId, x.Language });
            e.Property(x => x.Language).HasColumnType("varchar(10)").HasMaxLength(10).IsRequired();
            e.Property(x => x.Question).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Answer).IsRequired();

            e.HasOne(x => x.QaItem).WithMany(q => q.Translations)
                .HasForeignKey(x => x.QaId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
