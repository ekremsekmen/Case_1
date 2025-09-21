using Case_1_2.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Case_1_2.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.PasswordHash)
                    .IsRequired();
                
                entity.Property(e => e.FirstName)
                    .HasMaxLength(100);
                
                entity.Property(e => e.LastName)
                    .HasMaxLength(100);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure RefreshToken entity
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Token)
                    .IsRequired();
                
                entity.Property(e => e.ExpiryDate)
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Foreign key relationship
                entity.HasOne(e => e.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Stock)
                    .IsRequired();
                
                entity.Property(e => e.Category)
                    .HasMaxLength(50);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Sample Product 1",
                    Description = "This is a sample product for testing",
                    Price = 29.99m,
                    Stock = 100,
                    Category = "Electronics",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Sample Product 2",
                    Description = "Another sample product",
                    Price = 49.99m,
                    Stock = 50,
                    Category = "Books",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Product && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    ((Product)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
