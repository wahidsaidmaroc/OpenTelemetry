using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.Models;

namespace OpenTelemetryDemo.Data;

/// <summary>
/// DbContext Entity Framework Core pour la base SQL Server.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Price).HasPrecision(18, 2);
        });

        // Données de démonstration insérées automatiquement à la création de la base.
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "PC Portable Dell XPS 15",
                Description = "Ordinateur portable 15 pouces, Intel Core i7, 32 Go RAM, 1 To SSD",
                Price = 1899.99m,
                Stock = 12,
                CreatedAt = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 2,
                Name = "PC Fixe Gamer",
                Description = "Tour gamer, AMD Ryzen 9, RTX 4080, 64 Go RAM",
                Price = 2499.00m,
                Stock = 5,
                CreatedAt = new DateTime(2025, 2, 1, 9, 30, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 3,
                Name = "Écran 27 pouces 4K",
                Description = "Moniteur 27 pouces, résolution 4K UHD, dalle IPS",
                Price = 449.90m,
                Stock = 30,
                CreatedAt = new DateTime(2025, 2, 10, 14, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 4,
                Name = "Clavier mécanique",
                Description = "Clavier mécanique rétroéclairé, switches rouges",
                Price = 89.99m,
                Stock = 50,
                CreatedAt = new DateTime(2025, 3, 5, 11, 15, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 5,
                Name = "Souris sans fil",
                Description = "Souris ergonomique sans fil, capteur 16000 DPI",
                Price = 59.99m,
                Stock = 80,
                CreatedAt = new DateTime(2025, 3, 20, 16, 45, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 6,
                Name = "Docking Station USB-C",
                Description = "Station d'accueil USB-C, double écran, charge 100W",
                Price = 199.00m,
                Stock = 20,
                CreatedAt = new DateTime(2025, 4, 2, 8, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
