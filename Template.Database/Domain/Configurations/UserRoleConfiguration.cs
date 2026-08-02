using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> entity)
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
    }
}
