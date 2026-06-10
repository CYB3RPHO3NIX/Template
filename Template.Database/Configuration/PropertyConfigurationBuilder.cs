using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.Database.Configuration
{
    public class PropertyConfigurationBuilder<TProperty>
    {
        private readonly Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<TProperty> _builder;

        public PropertyConfigurationBuilder(Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<TProperty> builder)
        {
            _builder = builder;
        }

        public PropertyConfigurationBuilder<TProperty> Column(string columnName)
        {
            _builder.HasColumnName(columnName);
            return this;
        }

        public PropertyConfigurationBuilder<TProperty> Required()
        {
            _builder.IsRequired();
            return this;
        }

        public PropertyConfigurationBuilder<TProperty> MaxLength(int maxLength)
        {
            _builder.HasMaxLength(maxLength);
            return this;
        }

        public PropertyConfigurationBuilder<TProperty> Type(string columnType)
        {
            _builder.HasColumnType(columnType);
            return this;
        }

        public PropertyConfigurationBuilder<TProperty> Default(object value)
        {
            _builder.HasDefaultValue(value);
            return this;
        }

        public PropertyConfigurationBuilder<TProperty> DefaultSql(string sql)
        {
            _builder.HasDefaultValueSql(sql);
            return this;
        }

        public PropertyConfigurationBuilder<TProperty> GeneratedOnAdd()
        {
            _builder.ValueGeneratedOnAdd();
            return this;
        }

        public PropertyConfigurationBuilder<TProperty> AsStringEnum()
        {
            _builder.HasConversion<string>();
            return this;
        }
    }
}
