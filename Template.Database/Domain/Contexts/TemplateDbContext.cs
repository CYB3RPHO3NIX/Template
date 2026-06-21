using Microsoft.EntityFrameworkCore;
using Template.Database.Context;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Contexts
{
    public class TemplateDbContext : BaseDbContext
    {
        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
    }
}
