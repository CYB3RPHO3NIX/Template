using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> entity)
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
    }
}
