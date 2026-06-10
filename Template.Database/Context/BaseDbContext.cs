using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Template.Database.Context
{
    public abstract class BaseDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the BaseDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types.
        /// Automatically applies all IEntityTypeConfiguration implementations from the consuming assembly.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
