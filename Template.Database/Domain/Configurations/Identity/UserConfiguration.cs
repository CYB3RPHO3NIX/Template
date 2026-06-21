using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "identity");

            builder.HasKey(x => x.Id)
                .HasName("PK_Users");

            builder.Property(x => x.Id)
                .HasColumnOrder(1)
                .HasColumnType("uniqueidentifier")
                .ValueGeneratedNever();

            builder.Property(x => x.Username)
                .HasColumnOrder(2)
                .HasColumnType("varchar(256)");

            builder.Property(x => x.Email)
                .HasColumnOrder(3)
                .HasColumnType("varchar(512)");

            builder.Property(x => x.PasswordHash)
                .HasColumnOrder(4)
                .HasColumnType("varchar(1024)");

            builder.Property(x => x.PasswordSalt)
                .HasColumnOrder(5)
                .HasColumnType("varchar(1024)");

            builder.Property(x => x.IsActive)
                .HasColumnOrder(6)
                .HasColumnType("bit");

            builder.HasIndex(x => x.Username)
                .IsUnique()
                .HasDatabaseName("UX_Users_UserName");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("UX_Users_Email");
        }
    }
}
