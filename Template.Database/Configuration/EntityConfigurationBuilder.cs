using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.Database.Configuration
{
    public class EntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        private readonly EntityTypeBuilder<TEntity> _builder;

        public EntityConfigurationBuilder(EntityTypeBuilder<TEntity> builder)
        {
            _builder = builder;
        }

        public EntityConfigurationBuilder<TEntity> Table(string tableName, string? schema = null)
        {
            if (string.IsNullOrWhiteSpace(schema))
            {
                _builder.ToTable(tableName);
            }
            else
            {
                _builder.ToTable(tableName, schema);
            }

            return this;
        }

        public EntityConfigurationBuilder<TEntity> Key(Expression<Func<TEntity, object?>> keyExpression)
        {
            _builder.HasKey(keyExpression);
            return this;
        }

        public EntityConfigurationBuilder<TEntity> Index(Expression<Func<TEntity, object?>> indexExpression, bool isUnique = false, string? databaseName = null)
        {
            var indexBuilder = _builder.HasIndex(indexExpression);

            if (isUnique)
            {
                indexBuilder.IsUnique();
            }

            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                indexBuilder.HasDatabaseName(databaseName);
            }

            return this;
        }

        public EntityConfigurationBuilder<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, Action<PropertyConfigurationBuilder<TProperty>> configure)
        {
            configure(new PropertyConfigurationBuilder<TProperty>(_builder.Property(propertyExpression)));
            return this;
        }

        public EntityConfigurationBuilder<TEntity> OneToOne<TRelatedEntity>(
            Expression<Func<TEntity, TRelatedEntity?>> navigationExpression,
            Expression<Func<TRelatedEntity, TEntity?>> inverseExpression,
            Expression<Func<TRelatedEntity, object?>> foreignKeyExpression,
            DeleteBehavior deleteBehavior = DeleteBehavior.Restrict)
            where TRelatedEntity : class
        {
            _builder
                .HasOne(navigationExpression)
                .WithOne(inverseExpression)
                .HasForeignKey<TRelatedEntity>(foreignKeyExpression)
                .OnDelete(deleteBehavior);

            return this;
        }

        public EntityConfigurationBuilder<TEntity> OneToMany<TRelatedEntity>(
            Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> navigationExpression,
            Expression<Func<TRelatedEntity, TEntity?>> inverseExpression,
            Expression<Func<TRelatedEntity, object?>> foreignKeyExpression,
            DeleteBehavior deleteBehavior = DeleteBehavior.Restrict)
            where TRelatedEntity : class
        {
            _builder
                .HasMany(navigationExpression)
                .WithOne(inverseExpression)
                .HasForeignKey(foreignKeyExpression)
                .OnDelete(deleteBehavior);

            return this;
        }

        public EntityConfigurationBuilder<TEntity> ManyToOne<TRelatedEntity>(
            Expression<Func<TEntity, TRelatedEntity?>> navigationExpression,
            Expression<Func<TRelatedEntity, IEnumerable<TEntity>>> inverseExpression,
            Expression<Func<TEntity, object?>> foreignKeyExpression,
            DeleteBehavior deleteBehavior = DeleteBehavior.Restrict)
            where TRelatedEntity : class
        {
            _builder
                .HasOne(navigationExpression)
                .WithMany(inverseExpression)
                .HasForeignKey(foreignKeyExpression)
                .OnDelete(deleteBehavior);

            return this;
        }
    }
}
