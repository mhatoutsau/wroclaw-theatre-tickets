namespace WroclawTheatreTickets.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Domain.Entities;

public class TheatreDbContext : DbContext
{
    public TheatreDbContext(DbContextOptions<TheatreDbContext> options) : base(options) { }

    public DbSet<Theatre> Theatres => Set<Theatre>();
    public DbSet<Show> Shows => Set<Show>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    public DbSet<ViewHistory> ViewHistories => Set<ViewHistory>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Theatre configuration
        modelBuilder.Entity<Theatre>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100).HasDefaultValue("Wrocław");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(500);
            entity.Property(e => e.BookingUrl).HasMaxLength(500);
            entity.HasMany(e => e.Shows).WithOne(s => s.Theatre).HasForeignKey(s => s.TheatreId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });

        // Show configuration
        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Director).HasMaxLength(200);
            entity.Property(e => e.Cast).HasMaxLength(1000);
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.ExternalId).HasMaxLength(200);
            entity.HasOne(e => e.Theatre).WithMany(t => t.Shows).HasForeignKey(e => e.TheatreId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.UserFavorites).WithOne(f => f.Show).HasForeignKey(f => f.ShowId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Reviews).WithOne(r => r.Show).HasForeignKey(r => r.ShowId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.StartDateTime);
            entity.HasIndex(e => e.TheatreId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.ExternalId);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.ExternalId).HasMaxLength(200);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => new { e.ExternalId, e.Provider });
            entity.HasMany(e => e.Favorites).WithOne(f => f.User).HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.ViewHistory).WithOne(v => v.User).HasForeignKey(v => v.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Reviews).WithOne(r => r.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Notifications).WithOne(n => n.User).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // UserFavorite configuration
        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User).WithMany(u => u.Favorites).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Show).WithMany(s => s.UserFavorites).HasForeignKey(e => e.ShowId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.UserId, e.ShowId }).IsUnique();
        });

        // ViewHistory configuration
        modelBuilder.Entity<ViewHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User).WithMany(u => u.ViewHistory).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Show).WithMany().HasForeignKey(e => e.ShowId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.UserId, e.ViewedAt }).IsDescending(false, true);
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.HasOne(e => e.User).WithMany(u => u.Reviews).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Show).WithMany(s => s.Reviews).HasForeignKey(e => e.ShowId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.ShowId, e.IsApproved });
            entity.HasIndex(e => e.UserId);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Show).WithMany().HasForeignKey(e => e.ShowId).OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.UserId, e.IsRead });
            entity.HasIndex(e => e.CreatedAt).IsDescending();
        });
    }
}
