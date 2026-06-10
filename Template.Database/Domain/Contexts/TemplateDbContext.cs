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
        public DbSet<UserDetails> UserDetails => Set<UserDetails>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    }
}
