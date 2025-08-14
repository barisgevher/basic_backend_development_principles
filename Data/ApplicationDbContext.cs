namespace ProductAPI.Data
{
    using global::ProductAPI.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using System.Threading.Tasks;

    namespace ProductAPI.Data
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }

            public DbSet<Product> Products { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Product entity configuration
                modelBuilder.Entity<Product>(entity =>
                {
                    // Primary key
                    entity.HasKey(p => p.Id);

                    // Property configurations
                    entity.Property(p => p.Name)
                        .IsRequired()
                        .HasMaxLength(200);

                    entity.Property(p => p.Description)
                        .HasMaxLength(1000);

                    entity.Property(p => p.Price)
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();

                    entity.Property(p => p.StockQuantity)
                        .IsRequired();

                    entity.Property(p => p.Category)
                        .HasMaxLength(100);

                    entity.Property(p => p.Brand)
                        .HasMaxLength(50);

                    entity.Property(p => p.ImageUrl)
                        .HasMaxLength(500);

                    entity.Property(p => p.IsActive)
                        .HasDefaultValue(true);

                    entity.Property(p => p.CreatedAt)
                        .IsRequired()
                        .HasDefaultValueSql("GETUTCDATE()"); // SQL Server için
                                                             // .HasDefaultValueSql("CURRENT_TIMESTAMP"); // PostgreSQL için

                    entity.Property(p => p.UpdatedAt);

                    // Indexes for better performance
                    entity.HasIndex(p => p.Name)
                        .HasDatabaseName("IX_Products_Name");

                    entity.HasIndex(p => p.Category)
                        .HasDatabaseName("IX_Products_Category");

                    entity.HasIndex(p => p.Brand)
                        .HasDatabaseName("IX_Products_Brand");

                    entity.HasIndex(p => p.IsActive)
                        .HasDatabaseName("IX_Products_IsActive");

                    entity.HasIndex(p => p.CreatedAt)
                        .HasDatabaseName("IX_Products_CreatedAt");

                    // Composite index for common queries
                    entity.HasIndex(p => new { p.Category, p.IsActive })
                        .HasDatabaseName("IX_Products_Category_IsActive");
                });

                // Seed data
                SeedData(modelBuilder);
            }

            private static void SeedData(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Product>().HasData(
                    new Product
                    {
                        Id = 1,
                        Name = "Bohemian Style Rug",
                        Description = "A beautiful hand-woven bohemian style rug, perfect for living rooms and bedrooms. Adds a warm and cozy touch to any space.",
                        Price = 149.99m,
                        StockQuantity = 30,
                        Category = "Home Decor",
                        Brand = "HomeStyle",
                        ImageUrl = "https://example.com/images/rug.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Velvet Throw Pillow",
                        Description = "Soft and luxurious velvet throw pillow to add a touch of elegance to your sofa or bed. Available in multiple colors.",
                        Price = 29.99m,
                        StockQuantity = 150,
                        Category = "Textiles",
                        Brand = "ComfyHome",
                        ImageUrl = "https://example.com/images/pillow.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Waterproof Pet Mat",
                        Description = "Durable and waterproof mat for your pet's comfort and your floor's protection. Easy to clean and maintain.",
                        Price = 39.99m,
                        StockQuantity = 80,
                        Category = "Pet Supplies",
                        Brand = "PetCare",
                        ImageUrl = "https://example.com/images/petmat.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "Modern 3-Seater Sofa",
                        Description = "A sleek and modern 3-seater sofa with comfortable cushions and a sturdy wooden frame. Perfect for contemporary living rooms.",
                        Price = 799.99m,
                        StockQuantity = 15,
                        Category = "Furniture",
                        Brand = "UrbanLiving",
                        ImageUrl = "https://example.com/images/sofa.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 5,
                        Name = "Rustic Wooden Bench",
                        Description = "A rustic wooden bench, ideal for entryways, dining areas, or gardens. Made from solid reclaimed wood.",
                        Price = 129.99m,
                        StockQuantity = 25,
                        Category = "Furniture",
                        Brand = "HomeStyle",
                        ImageUrl = "https://example.com/images/bench.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Wicker Supla Placemat Set",
                        Description = "Set of 4 natural wicker supla placemats to protect your dining table and add a rustic charm to your meals.",
                        Price = 49.99m,
                        StockQuantity = 120,
                        Category = "Kitchen & Dining",
                        Brand = "DecorArt",
                        ImageUrl = "https://example.com/images/supla.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 7,
                        Name = "Abstract Metal Wall Decor",
                        Description = "Modern abstract metal art to create a stunning focal point in any room. Lightweight and easy to hang.",
                        Price = 89.99m,
                        StockQuantity = 40,
                        Category = "Home Decor",
                        Brand = "DecorArt",
                        ImageUrl = "https://example.com/images/walldecor.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 8,
                        Name = "Non-Slip Step Rug",
                        Description = "Absorbent and non-slip step rug, perfect for bathrooms, kitchens, or doorways. Machine washable.",
                        Price = 24.99m,
                        StockQuantity = 90,
                        Category = "Bath",
                        Brand = "ComfyHome",
                        ImageUrl = "https://example.com/images/steprug.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 9,
                        Name = "Luxury Cotton Towel Set",
                        Description = "A set of 2 luxury bath towels made from 100% premium Turkish cotton. Ultra-soft and highly absorbent.",
                        Price = 59.99m,
                        StockQuantity = 110,
                        Category = "Bath",
                        Brand = "ComfyHome",
                        ImageUrl = "https://example.com/images/towel.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 10,
                        Name = "Classic Christmas Stocking",
                        Description = "A classic red and white Christmas stocking, ready to be filled with gifts and treats. Perfect for the holiday season.",
                        Price = 19.99m,
                        StockQuantity = 200,
                        Category = "Seasonal Decor",
                        Brand = "HolidayJoy",
                        ImageUrl = "https://example.com/images/stocking.jpg",
                        IsActive = false, // Example of an inactive product
                        CreatedAt = DateTime.UtcNow
                    }
                );
            }

            // Override SaveChanges to automatically set UpdatedAt
            public override int SaveChanges()
            {
                UpdateTimestamps();
                return base.SaveChanges();
            }

            public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                UpdateTimestamps();
                return await base.SaveChangesAsync(cancellationToken);
            }

            private void UpdateTimestamps()
            {
                var entries = ChangeTracker.Entries()
                    .Where(e => e.Entity is Product && (e.State == EntityState.Modified));

                foreach (var entry in entries)
                {
                    if (entry.Entity is Product product)
                    {
                        product.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
