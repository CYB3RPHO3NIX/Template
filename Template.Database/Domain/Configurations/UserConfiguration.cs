using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
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
    }
}
