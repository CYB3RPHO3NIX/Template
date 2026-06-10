using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.Database.Configuration
{
    public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            Configure(new EntityConfigurationBuilder<TEntity>(builder));
        }

        protected abstract void Configure(EntityConfigurationBuilder<TEntity> builder);
    }
}
