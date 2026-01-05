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

            user.HasIndex(u => u.Id).IsUnique();
        });

        modelBuilder.Entity<UserSession>(sessoin =>
        {
            sessoin.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            sessoin.OwnsOne(s => s.RefreshToken, rt => { rt.HasIndex(x => x.TokenHashed).IsUnique(); });

            sessoin.HasIndex(s => s.UserId);
            sessoin.HasIndex(s => s.Id).IsUnique();
            sessoin.HasIndex(s => s.DeviceId);
        });

        modelBuilder.Entity<UserRelation>(relation =>
            {
                relation.HasKey(x => new
                {
                    x.UserId,
                    x.TargetUserId,
                    x.RelationType
                });

                relation.Property(x => x.RelationType)
                    .HasMaxLength(32)
                    .IsRequired();

                relation.Property(x => x.Status)
                    .HasMaxLength(32);

                relation.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                relation.HasOne(x => x.TargetUser)
                    .WithMany()
                    .HasForeignKey(x => x.TargetUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                relation.Property(x => x.RelationType)
                    .HasConversion<string>()
                    .HasMaxLength(32)
                    .IsRequired();

                relation.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(32)
                    .IsRequired();

                relation.HasIndex(x => x.UserId);
                relation.HasIndex(x => x.TargetUserId);
                relation.HasIndex(x => new { x.UserId, x.RelationType, x.Status });
            }
        );


        base.OnModelCreating(modelBuilder);
    }
}