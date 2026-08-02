using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> entity)
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
    }
}
