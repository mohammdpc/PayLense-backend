using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayLense.Domain.Entities;
using PayLense.Infrastructure.Identity;

namespace PayLense.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BankCard> BankCards => Set<BankCard>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TrustedDevice> TrustedDevices => Set<TrustedDevice>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BankCard>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.CardNumberEncrypted).IsRequired();
            e.Property(x => x.Last4Digits).HasMaxLength(4).IsRequired();
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

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
    }
}
