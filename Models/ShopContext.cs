using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Shop.Models;

public partial class ShopContext : DbContext
{
    public ShopContext()
    {
    }

    public ShopContext(DbContextOptions<ShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MSI;Initial Catalog=shop;Integrated Security=True;Connect Timeout=30; Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.DateOfLastDelivery)
                .HasColumnType("date")
                .HasColumnName("Date_Of_Last_Delivery");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PriceUnit)
                .HasColumnType("money")
                .HasColumnName("Price_Unit");
            entity.Property(e => e.UnitOfMeasurement)
                .HasMaxLength(10)
                .HasColumnName("Unit_Of_Measurement");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sale");

            entity.Property(e => e.SaleId).HasColumnName("Sale_ID");
            entity.Property(e => e.AmountOfProducts).HasColumnName("Amount_Of_Products");
            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.SellerId).HasColumnName("Seller_ID");

            entity.HasOne(d => d.Product).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Sale_Product");

            entity.HasOne(d => d.Seller).WithMany(p => p.Sales)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK_Sale_Seller");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.ToTable("Seller");

            entity.Property(e => e.SellerId).HasColumnName("Seller_ID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("First_Name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("Last_Name");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(10);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.ToTable("Storage");

            entity.Property(e => e.StorageId).HasColumnName("Storage_ID");
            entity.Property(e => e.DateDelivery)
                .HasColumnType("date")
                .HasColumnName("Date_Delivery");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.QuantityOfProducts).HasColumnName("Quantity_Of_Products");
            entity.Property(e => e.TotalCost)
                .HasColumnType("money")
                .HasColumnName("Total_Cost");

            entity.HasOne(d => d.Product).WithMany(p => p.Storages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Storage_Product");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
