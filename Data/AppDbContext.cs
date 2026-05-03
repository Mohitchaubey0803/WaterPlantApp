using Microsoft.EntityFrameworkCore;
using WaterPlantApp.Models;

namespace WaterPlantApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoreProduct> StoreProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasIndex(e => e.StoreCode).IsUnique();
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.StoreType).HasDefaultValue("Retail");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<StoreProduct>(entity =>
            {
                entity.HasIndex(e => new { e.StoreId, e.ProductId }).IsUnique();
                entity.HasOne(sp => sp.Store).WithMany(s => s.StoreProducts).HasForeignKey(sp => sp.StoreId);
                entity.HasOne(sp => sp.Product).WithMany(p => p.StoreProducts).HasForeignKey(sp => sp.ProductId);
            });
        }
    }
}
