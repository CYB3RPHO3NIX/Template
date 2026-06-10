using Template.Database.Configuration;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class RoleConfiguration : EntityConfiguration<Role>
    {
        protected override void Configure(EntityConfigurationBuilder<Role> builder)
        {
            builder
                .Table("Roles", "identity")
                .Key(role => role.Id)
                .Property(role => role.Id, property => property.Column("RoleId").GeneratedOnAdd())
                .Property(role => role.Name, property => property.Column("RoleName").Required().MaxLength(100))
                .Property(role => role.Description, property => property.Column("Description").MaxLength(500))
                .Index(role => role.Name, isUnique: true, databaseName: "IX_Roles_RoleName")
                .OneToMany(role => role.UserRoles, userRole => userRole.Role, userRole => userRole.RoleId)
                .OneToMany(role => role.RolePermissions, rolePermission => rolePermission.Role, rolePermission => rolePermission.RoleId);
        }
    }
}
