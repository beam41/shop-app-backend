﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ShopAppBackend.Models;

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
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<OrderState>()
                .Property(os => os.CreatedAt)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Product>()
                .Property(p => p.Archived)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductType>()
                .Property(pt => pt.Archived)
                .HasDefaultValue(false);

            modelBuilder.Entity<DistributionMethod>()
                .Property(dm => dm.Archived)
                .HasDefaultValue(false);

            modelBuilder.Entity<Promotion>()
                .Property(p => p.Archived)
                .HasDefaultValue(false);

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

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DistributionMethod)
                .WithMany(d => d.Orders);

            modelBuilder.Entity<BuildOrder>()
                .HasOne(bo => bo.DistributionMethod)
                .WithMany(d => d.BuildOrders);

            modelBuilder.Entity<BuildOrder>()
                .HasMany(bo => bo.OrderStates)
                .WithOne(os => os.BuildOrder);

            modelBuilder.Entity<BuildOrder>()
                .HasMany(bo => bo.DescriptionImages)
                .WithOne(oi => oi.BuildOrder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.BuildOrders)
                .WithOne(bo => bo.CreatedByUser);
        }

        public DbSet<User> User { get; set; }

        public DbSet<ProductType> ProductType { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Promotion> Promotion { get; set; }

        public DbSet<PromotionItem> PromotionItem { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderProduct> OrderProduct { get; set; }

        public DbSet<PaymentMethod> PaymentMethod { get; set; }

        public DbSet<DistributionMethod> DistributionMethod { get; set; }

        public DbSet<BuildOrder> BuildOrder { get; set; }

    }
}
