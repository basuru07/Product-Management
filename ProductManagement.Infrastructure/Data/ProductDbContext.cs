using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) {  }

        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            { 
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                .HasColumnName("NAME")
                .IsRequired()
                .HasMaxLength(100);

                entity.Property(e => e.Description)
                .HasColumnName("DESCRIPTION")
                .IsRequired()
                .HasMaxLength(100);

                entity.Property(e => e.Price)
                .HasColumnName("PRICE")
                .HasColumnType("decimal(18,2)");

                entity.Property(e => e.StockQuantity)
                .HasColumnName("STOCK_QUANTITY")
                .IsRequired();

                entity.Property(e => e.CreatedDate)
                .HasColumnName("CREATED_DATE")
                .IsRequired();

                entity.Property(e => e.UpdatedDate)
                .HasColumnName("UPDATED_DATE")
                .IsRequired(false);

            });
            base.OnModelCreating(modelBuilder);

        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entires = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Product &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));
            foreach (var entityEntry in entires)
            {

                if (entityEntry.State == EntityState.Added)
                {
                    ((Product)entityEntry.Entity).CreatedDate = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    ((Product)entityEntry.Entity).UpdatedDate = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }   
}
