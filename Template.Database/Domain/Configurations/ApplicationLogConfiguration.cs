using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class ApplicationLogConfiguration : IEntityTypeConfiguration<ApplicationLog>
{
    public void Configure(EntityTypeBuilder<ApplicationLog> entity)
    {
        entity.ToTable("ApplicationLogs", "log");

        entity.Property(e => e.Level).HasMaxLength(16);
        entity.Property(e => e.TimeStamp).HasColumnType("datetime");
    }
}
