using Template.Database.Configuration;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class UserRoleConfiguration : EntityConfiguration<UserRole>
    {
        protected override void Configure(EntityConfigurationBuilder<UserRole> builder)
        {
            builder
                .Table("UserRoles", "identity")
                .Key(userRole => userRole.Id)
                .Property(userRole => userRole.Id, property => property.Column("UserRoleId").GeneratedOnAdd())
                .Property(userRole => userRole.UserId, property => property.Column("UserId").Required())
                .Property(userRole => userRole.RoleId, property => property.Column("RoleId").Required())
                .Index(userRole => new { userRole.UserId, userRole.RoleId }, isUnique: true, databaseName: "IX_UserRoles_UserId_RoleId")
                .ManyToOne(userRole => userRole.User, user => user.UserRoles, userRole => userRole.UserId)
                .ManyToOne(userRole => userRole.Role, role => role.UserRoles, userRole => userRole.RoleId);
        }
    }
}
