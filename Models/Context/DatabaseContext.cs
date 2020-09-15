﻿using Microsoft.EntityFrameworkCore;

namespace ShopAppBackend.Models.Context
{
    public sealed class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username);

            // product(M) and its Type(1)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Type)
                .WithMany(t => t.Products);

            // product(1) and its image(M)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product);

            // promotion(M) and product(M)
            modelBuilder.Entity<PromotionItem>()
                .HasOne(pi => pi.Promotion)
                .WithMany(p => p.PromotionItems);

            modelBuilder.Entity<PromotionItem>()
                .HasOne(pi => pi.InPromotionProduct)
                .WithMany(p => p.PromotionItems);

            // user(1) and their order(M)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.CreatedByUser);

            // order(1) and their state(M)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderStates)
                .WithOne(os => os.Order);

            // order(M) and product(M)
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(p => p.OrderProducts);

            // order(M) and promotion(M)
            modelBuilder.Entity<OrderPromotion>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderPromotions);

            modelBuilder.Entity<OrderPromotion>()
                .HasOne(op => op.Promotion)
                .WithMany(p => p.OrderPromotions);
        }

        public DbSet<User> User { get; set; }

        public DbSet<ProductType> ProductType { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<ProductImage> ProductImage { get; set; }

        public DbSet<Promotion> Promotion { get; set; }

        public DbSet<PromotionItem> PromotionItem { get; set; }

        public DbSet<PromotionItem> Order { get; set; }

        public DbSet<OrderState> OrderStates { get; set; }

        public DbSet<PromotionItem> OrderProduct { get; set; }

        public DbSet<PromotionItem> OrderPromotion { get; set; }

    }
}
