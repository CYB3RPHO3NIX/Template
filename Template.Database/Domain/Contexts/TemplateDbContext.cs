using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Contexts;

public partial class TemplateDbContext : DbContext
{
    public TemplateDbContext(DbContextOptions<TemplateDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationLog> ApplicationLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationLog>(entity =>
        {
            entity.ToTable("ApplicationLogs", "log");

            entity.Property(e => e.Level).HasMaxLength(16);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "identity");

            entity.HasIndex(e => e.Email, "UX_Users_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UX_Users_UserName").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email)
                .HasMaxLength(512)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(1024)
                .IsUnicode(false);
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(1024)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(256)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
