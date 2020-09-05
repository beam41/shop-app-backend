using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopAppBackend.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ShopAppBackend.Models.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Type)
                .WithMany(t => t.Products);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product);
        }

        public DbSet<User> User { get; set; }

        public DbSet<ProductType> ProductType { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<ProductImage> ProductImage { get; set; }
        
    }
}
