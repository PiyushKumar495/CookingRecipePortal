using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipePortal.Models;

public partial class CookingDbContext : DbContext
{
    public CookingDbContext()
    {
    }

    public CookingDbContext(DbContextOptions<CookingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD6B5B8A029");

            entity.ToTable("Feedback");

            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Cascade delete when Recipe is deleted
            entity.HasOne(d => d.Recipe).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade) 
                .HasConstraintName("FK_Feedback_Recipes");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
             .HasForeignKey(d => d.UserId)
             .OnDelete(DeleteBehavior.ClientSetNull) // or Restrict
                        .HasConstraintName("FK_Feedback_Users");


        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988B0E7C99165");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Ingredients).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Instructions).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ImageUrl).IsRequired(false); 

            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);

            // Cascade delete when User is deleted
            entity.HasOne(d => d.User).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade) 
                .HasConstraintName("FK_Recipes_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC0EDABBC");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CCA24788").IsUnique();

            entity.Property(e => e.Bio).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role).HasDefaultValue("User").HasMaxLength(10); 
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
