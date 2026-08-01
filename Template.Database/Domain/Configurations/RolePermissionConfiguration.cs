using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> entity)
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
    }
}
