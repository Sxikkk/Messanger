using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance;

public class MyAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<UserRelation> UserRelations { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            user.HasIndex(u => u.Email).IsUnique();
            user.HasIndex(u => u.Login).IsUnique();
            user.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<UserSession>(session =>
        {
            session.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            session.OwnsOne(s => s.RefreshToken, rt =>
            {
                rt.Property(x => x.TokenHashed).HasColumnName("RefreshTokenHashed");
                rt.Property(x => x.ExpiresAt).HasColumnName("RefreshTokenExpiresAt");
                rt.HasIndex(x => x.TokenHashed).IsUnique();
                rt.HasIndex(x => x.ExpiresAt);
            });

            session.HasIndex(s => s.UserId);
            session.HasIndex(s => s.Id).IsUnique();
            session.HasIndex(s => s.DeviceId);
            session.HasIndex(s => new { s.UserId, s.DeviceId }).IsUnique();
        });

        modelBuilder.Entity<UserRelation>(relation =>
        {
            relation.HasKey(x => new { x.UserId, x.TargetUserId, RelationType = x.ERelationType });

            relation.Property(x => x.ERelationType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            relation.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            relation.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            relation.HasOne(x => x.TargetUser)
                .WithMany()
                .HasForeignKey(x => x.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);

            relation.HasIndex(x => x.UserId);
            relation.HasIndex(x => x.TargetUserId);
            relation.HasIndex(x => new { x.UserId, RelationType = x.ERelationType, x.Status });
        });

        base.OnModelCreating(modelBuilder);
    }
}