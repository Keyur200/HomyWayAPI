﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HomyWayAPI.Models;

public partial class HomyWayContext : DbContext
{
    public HomyWayContext()
    {
    }

    public HomyWayContext(DbContextOptions<HomyWayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<PropertyCategoryTbl> PropertyCategoryTbls { get; set; }

    public virtual DbSet<PropertyTbl> PropertyTbls { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-757AVSO\\SQLEXPRESS;Initial Catalog=HomyWay;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("groups");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<PropertyCategoryTbl>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("property_categoryTBL");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<PropertyTbl>(entity =>
        {
            entity.HasKey(e => e.PropertyId);

            entity.ToTable("propertyTBL");

            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.Bathroom).HasColumnName("bathroom");
            entity.Property(e => e.Bed).HasColumnName("bed");
            entity.Property(e => e.BedRoom).HasColumnName("bed_room");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.HostId).HasColumnName("host_id");
            entity.Property(e => e.MaxGuests).HasColumnName("max_guests");
            entity.Property(e => e.PropertyAdderss)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("property_adderss");
            entity.Property(e => e.PropertyCity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("property_city");
            entity.Property(e => e.PropertyCountry)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("property_country");
            entity.Property(e => e.PropertyDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("property_description");
            entity.Property(e => e.PropertyName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("property_name");
            entity.Property(e => e.PropertyPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("property_price");
            entity.Property(e => e.PropertyState)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("property_state");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Category).WithMany(p => p.PropertyTbls)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_propertyTBL_property_categoryTBL");

            entity.HasOne(d => d.Host).WithMany(p => p.PropertyTbls)
                .HasForeignKey(d => d.HostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_propertyTBL_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
