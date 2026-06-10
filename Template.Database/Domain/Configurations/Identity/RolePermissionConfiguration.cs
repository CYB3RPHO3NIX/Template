using Template.Database.Configuration;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class RolePermissionConfiguration : EntityConfiguration<RolePermission>
    {
        protected override void Configure(EntityConfigurationBuilder<RolePermission> builder)
        {
            builder
                .Table("RolePermissions", "identity")
                .Key(rolePermission => rolePermission.Id)
                .Property(rolePermission => rolePermission.Id, property => property.Column("RolePermissionId").GeneratedOnAdd())
                .Property(rolePermission => rolePermission.RoleId, property => property.Column("RoleId").Required())
                .Property(rolePermission => rolePermission.PermissionId, property => property.Column("PermissionId").Required())
                .Index(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId }, isUnique: true, databaseName: "IX_RolePermissions_RoleId_PermissionId")
                .ManyToOne(rolePermission => rolePermission.Role, role => role.RolePermissions, rolePermission => rolePermission.RoleId)
                .ManyToOne(rolePermission => rolePermission.Permission, permission => permission.RolePermissions, rolePermission => rolePermission.PermissionId);
        }
    }
}
