using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
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
    }
}
