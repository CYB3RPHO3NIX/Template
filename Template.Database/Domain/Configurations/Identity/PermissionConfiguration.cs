using Template.Database.Configuration;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class PermissionConfiguration : EntityConfiguration<Permission>
    {
        protected override void Configure(EntityConfigurationBuilder<Permission> builder)
        {
            builder
                .Table("Permissions", "identity")
                .Key(permission => permission.Id)
                .Property(permission => permission.Id, property => property.Column("PermissionId").GeneratedOnAdd())
                .Property(permission => permission.Name, property => property.Column("PermissionName").Required().MaxLength(100))
                .Property(permission => permission.Description, property => property.Column("Description").MaxLength(500))
                .Index(permission => permission.Name, isUnique: true, databaseName: "IX_Permissions_PermissionName")
                .OneToMany(permission => permission.RolePermissions, rolePermission => rolePermission.Permission, rolePermission => rolePermission.PermissionId)
                .OneToMany(permission => permission.UserPermissions, userPermission => userPermission.Permission, userPermission => userPermission.PermissionId);
        }
    }
}
