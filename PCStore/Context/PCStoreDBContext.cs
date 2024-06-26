﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCStore.Models;

namespace PCStore.Context;

public partial class PCStoreDBContext : IdentityDbContext<User>
{
    public PCStoreDBContext(DbContextOptions<PCStoreDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ShippingInfo> ShippingInfos { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<ShoppingCartProduct> ShoppingCartProducts { get; set; }

    public virtual DbSet<Spec> Specs { get; set; }

    public virtual DbSet<SpecsOption> SpecsOptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("addresses");

            entity.HasIndex(e => e.UserId, "UserId_idx");

            entity.Property(e => e.Apartment).HasMaxLength(45);
            entity.Property(e => e.City).HasMaxLength(45);
            entity.Property(e => e.Country).HasMaxLength(45);
            entity.Property(e => e.PostCode).HasMaxLength(45);
            entity.Property(e => e.Province).HasMaxLength(45);
            entity.Property(e => e.Street).HasMaxLength(45);

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.StatusId, "StatusId_idx");

            entity.HasIndex(e => e.UserId, "UserId1_idx");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StatusId");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserId2");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("order_products");

            entity.HasIndex(e => e.OrderId, "OrderId_idx");

            entity.HasIndex(e => e.ProductId, "ProductId1_idx");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderId");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ProductId3");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("order_statuses");

            entity.Property(e => e.Status).HasMaxLength(45);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.CategoryId, "CategoryId_idx");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(45);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CategoryId");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_categories");

            entity.Property(e => e.Name).HasMaxLength(45);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_images");

            entity.HasIndex(e => e.ProductId, "ProductId_idx");

            entity.Property(e => e.Url).HasMaxLength(100);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ProductId");
        });

        modelBuilder.Entity<ShippingInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shipping_info");

            entity.HasIndex(e => e.OrderId, "OrderId1_idx");

            entity.Property(e => e.Apartment).HasMaxLength(45);
            entity.Property(e => e.City).HasMaxLength(45);
            entity.Property(e => e.Country).HasMaxLength(45);
            entity.Property(e => e.PostCode).HasMaxLength(45);
            entity.Property(e => e.Province).HasMaxLength(45);
            entity.Property(e => e.Street).HasMaxLength(45);

            entity.HasOne(d => d.Order).WithMany(p => p.ShippingInfos)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderId1");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shopping_carts");

            entity.HasIndex(e => e.UserId, "UserId1_idx1");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserId1");
        });

        modelBuilder.Entity<ShoppingCartProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shopping_cart_products");

            entity.HasIndex(e => e.CartId, "CartId_idx");

            entity.HasIndex(e => e.ProdcutId, "ProductId1_idx1");

            entity.HasOne(d => d.Cart).WithMany(p => p.ShoppingCartProducts)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CartId");

            entity.HasOne(d => d.Prodcut).WithMany(p => p.ShoppingCartProducts)
                .HasForeignKey(d => d.ProdcutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ProductId2");
        });

        modelBuilder.Entity<Spec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("specs");

            entity.HasIndex(e => e.CategoryId, "CategoryId_idx1");

            entity.Property(e => e.Name).HasMaxLength(45);

            entity.HasOne(d => d.Category).WithMany(p => p.Specs)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CategoryId1");
        });

        modelBuilder.Entity<SpecsOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("specs_options");

            entity.HasIndex(e => e.ProductId, "ProductId1_idx2");

            entity.HasIndex(e => e.SpecId, "SpecId_idx");

            entity.Property(e => e.Value).HasMaxLength(45);

            entity.HasOne(d => d.Product).WithMany(p => p.SpecsOptions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ProductId1");

            entity.HasOne(d => d.Spec).WithMany(p => p.SpecsOptions)
                .HasForeignKey(d => d.SpecId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SpecId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
