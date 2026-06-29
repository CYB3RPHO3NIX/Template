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

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationLog>(entity =>
        {
            entity.ToTable("ApplicationLogs", "log");

            entity.Property(e => e.Level).HasMaxLength(16);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions", "identity");

            entity.HasIndex(e => e.Category, "IX_Permissions_Category");

            entity.HasIndex(e => e.PermissionCode, "IX_Permissions_Code");

            entity.HasIndex(e => e.ParentPermissionId, "IX_Permissions_Parent");

            entity.HasIndex(e => e.PermissionCode, "UQ_Permissions_Code").IsUnique();

            entity.Property(e => e.PermissionId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PermissionCode).HasMaxLength(100);
            entity.Property(e => e.PermissionName).HasMaxLength(100);
            entity.Property(e => e.UpdatedOn).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PermissionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Permissions_CreatedBy");

            entity.HasOne(d => d.ParentPermission).WithMany(p => p.InverseParentPermission)
                .HasForeignKey(d => d.ParentPermissionId)
                .HasConstraintName("FK_Permissions_Parent");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PermissionUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Permissions_UpdatedBy");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles", "identity");

            entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName").IsUnique();

            entity.Property(e => e.RoleId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.UpdatedOn).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Roles_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoleUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Roles_UpdatedBy");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions", "identity");

            entity.HasIndex(e => e.PermissionId, "IX_RolePermissions_PermissionId");

            entity.HasIndex(e => e.RoleId, "IX_RolePermissions_RoleId");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "UQ_RolePermissions").IsUnique();

            entity.Property(e => e.RolePermissionId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsAllowed).HasDefaultValue(true);

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_RolePermissions_AssignedBy");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK_RolePermissions_PermissionId");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RolePermissions_RoleId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "identity");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.UpdatedOn).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Users_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Users_UpdatedBy");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("UserPermissions", "identity");

            entity.HasIndex(e => e.PermissionId, "IX_UserPermissions_PermissionId");

            entity.HasIndex(e => e.UserId, "IX_UserPermissions_UserId");

            entity.HasIndex(e => new { e.UserId, e.PermissionId }, "UQ_UserPermissions").IsUnique();

            entity.Property(e => e.UserPermissionId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.UserPermissionAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_UserPermissions_AssignedBy");

            entity.HasOne(d => d.Permission).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK_UserPermissions_PermissionId");

            entity.HasOne(d => d.User).WithMany(p => p.UserPermissionUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserPermissions_UserId");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles", "identity");

            entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");

            entity.HasIndex(e => e.UserId, "IX_UserRoles_UserId");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ_UserRoles_UserId_RoleId").IsUnique();

            entity.Property(e => e.UserRoleId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.UserRoleAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_UserRoles_AssignedBy");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_RoleId");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoleUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_UserId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
