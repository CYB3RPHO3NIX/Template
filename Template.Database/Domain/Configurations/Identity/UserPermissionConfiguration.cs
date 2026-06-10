using Template.Database.Configuration;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class UserPermissionConfiguration : EntityConfiguration<UserPermission>
    {
        protected override void Configure(EntityConfigurationBuilder<UserPermission> builder)
        {
            builder
                .Table("UserPermissions", "identity")
                .Key(userPermission => userPermission.Id)
                .Property(userPermission => userPermission.Id, property => property.Column("UserPermissionId").GeneratedOnAdd())
                .Property(userPermission => userPermission.UserId, property => property.Column("UserId").Required())
                .Property(userPermission => userPermission.PermissionId, property => property.Column("PermissionId").Required())
                .Property(userPermission => userPermission.Access, property => property.Column("AccessFlag").Required().AsStringEnum().MaxLength(16))
                .Index(userPermission => new { userPermission.UserId, userPermission.PermissionId }, isUnique: true, databaseName: "IX_UserPermissions_UserId_PermissionId")
                .ManyToOne(userPermission => userPermission.User, user => user.UserPermissions, userPermission => userPermission.UserId)
                .ManyToOne(userPermission => userPermission.Permission, permission => permission.UserPermissions, userPermission => userPermission.PermissionId);
        }
    }
}
