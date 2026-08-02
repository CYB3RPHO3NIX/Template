using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class ApiLogConfiguration : IEntityTypeConfiguration<ApiLog>
{
    public void Configure(EntityTypeBuilder<ApiLog> entity)
    {
        entity.ToTable("ApiLogs", "api");

        entity.HasIndex(e => e.TraceId, "IX_ApiLogs_TraceId");

        entity.HasIndex(e => e.UserId, "IX_ApiLogs_UserId");

        entity.HasIndex(e => e.CreatedOn, "IX_ApiLogs_CreatedOn");

        entity.Property(e => e.ApiLogId).HasDefaultValueSql("(newsequentialid())");
        entity.Property(e => e.TraceId).HasMaxLength(100);
        entity.Property(e => e.Method).HasMaxLength(10);
        entity.Property(e => e.Path).HasMaxLength(2000);
        entity.Property(e => e.QueryString).HasMaxLength(2000);
        entity.Property(e => e.UserAgent).HasMaxLength(500);
        entity.Property(e => e.IpAddress).HasMaxLength(45);
        entity.Property(e => e.RequestHeaders).HasColumnType("nvarchar(max)");
        entity.Property(e => e.RequestBody).HasColumnType("nvarchar(max)");
        entity.Property(e => e.ResponseBody).HasColumnType("nvarchar(max)");
        entity.Property(e => e.Exception).HasColumnType("nvarchar(max)");
        entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
    }
}
