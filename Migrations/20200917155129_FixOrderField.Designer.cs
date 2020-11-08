// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShopAppBackend.Models.Context;

namespace ShopAppBackend.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20200917155129_FixOrderField")]
    partial class FixOrderField
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ShopAppBackend.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("PurchaseMethod")
                        .IsRequired()
                        .HasColumnType("varchar(11)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("ShopAppBackend.Models.OrderProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderProduct");
                });

            modelBuilder.Entity("ShopAppBackend.Models.OrderPromotion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("PromotionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("PromotionId");

                    b.ToTable("OrderPromotion");
                });

            modelBuilder.Entity("ShopAppBackend.Models.OrderState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("varchar(33)");

                    b.Property<string>("StateDataJson")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderStates");
                });

            modelBuilder.Entity("ShopAppBackend.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("ShopAppBackend.Models.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImageFileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImage");
                });

            modelBuilder.Entity("ShopAppBackend.Models.ProductType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductType");
                });

            modelBuilder.Entity("ShopAppBackend.Models.Promotion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsBroadcasted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Promotion");
                });

            modelBuilder.Entity("ShopAppBackend.Models.PromotionItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("InPromotionProductId")
                        .HasColumnType("int");

                    b.Property<int>("NewPrice")
                        .HasColumnType("int");

                    b.Property<int>("PromotionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InPromotionProductId");

                    b.HasIndex("PromotionId");

                    b.ToTable("PromotionItem");
                });

            modelBuilder.Entity("ShopAppBackend.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("District")
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("char(44)")
                        .HasMaxLength(44);

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("char(10)")
                        .HasMaxLength(10);

                    b.Property<string>("PostalCode")
                        .HasColumnType("char(5)")
                        .HasMaxLength(5);

                    b.Property<string>("Province")
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("SubDistrict")
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("ShopAppBackend.Models.Order", b =>
                {
                    b.HasOne("ShopAppBackend.Models.User", "CreatedByUser")
                        .WithMany("Orders")
                        .HasForeignKey("CreatedByUserId");
                });

            modelBuilder.Entity("ShopAppBackend.Models.OrderProduct", b =>
                {
                    b.HasOne("ShopAppBackend.Models.Order", "Order")
                        .WithMany("OrderProducts")
                        .HasForeignKey("OrderId");

                    b.HasOne("ShopAppBackend.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("ShopAppBackend.Models.OrderPromotion", b =>
                {
                    b.HasOne("ShopAppBackend.Models.Order", "Order")
                        .WithMany("OrderPromotions")
                        .HasForeignKey("OrderId");

                    b.HasOne("ShopAppBackend.Models.Promotion", "Promotion")
                        .WithMany("OrderPromotions")
                        .HasForeignKey("PromotionId");
                });

            modelBuilder.Entity("ShopAppBackend.Models.OrderState", b =>
                {
                    b.HasOne("ShopAppBackend.Models.Order", "Order")
                        .WithMany("OrderStates")
                        .HasForeignKey("OrderId");
                });

            modelBuilder.Entity("ShopAppBackend.Models.Product", b =>
                {
                    b.HasOne("ShopAppBackend.Models.ProductType", "Type")
                        .WithMany("Products")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAppBackend.Models.ProductImage", b =>
                {
                    b.HasOne("ShopAppBackend.Models.Product", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopAppBackend.Models.PromotionItem", b =>
                {
                    b.HasOne("ShopAppBackend.Models.Product", "InPromotionProduct")
                        .WithMany("PromotionItems")
                        .HasForeignKey("InPromotionProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShopAppBackend.Models.Promotion", "Promotion")
                        .WithMany("PromotionItems")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
