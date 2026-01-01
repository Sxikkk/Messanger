using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance;

public class MyAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}