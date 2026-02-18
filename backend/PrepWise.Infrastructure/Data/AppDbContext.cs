using Microsoft.EntityFrameworkCore;
using PrepWise.Domain.Entities;

namespace PrepWise.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Question> Questions => Set<Question>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).IsRequired();
            entity.Property(x => x.Difficulty).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(x => x.Tags).HasMaxLength(500);
            entity.Property(x => x.CreatedAt).IsRequired();
        });
    }
}
