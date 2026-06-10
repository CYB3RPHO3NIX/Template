using Microsoft.EntityFrameworkCore;
using Template.Database.Configuration;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class UserConfiguration : EntityConfiguration<User>
    {
        protected override void Configure(EntityConfigurationBuilder<User> builder)
        {
            builder
                .Table("Users", "identity")
                .Key(user => user.Id)
                .Property(user => user.Id, property => property.Column("UserId").GeneratedOnAdd())
                .Property(user => user.UserName, property => property.Column("Username").Required().MaxLength(100))
                .Property(user => user.Email, property => property.Column("EmailAddress").Required().MaxLength(256))
                .Property(user => user.PasswordHash, property => property.Column("PasswordHash").Required().MaxLength(512))
                .Property(user => user.PasswordSalt, property => property.Column("PasswordSalt").Required().MaxLength(256))
                .Property(user => user.IsActive, property => property.Column("IsActive").Default(true))
                .Index(user => user.UserName, isUnique: true, databaseName: "IX_Users_Username")
                .Index(user => user.Email, isUnique: true, databaseName: "IX_Users_EmailAddress")
                .OneToOne(user => user.UserDetails, details => details.User, details => details.UserId, DeleteBehavior.Cascade)
                .OneToMany(user => user.UserRoles, userRole => userRole.User, userRole => userRole.UserId, DeleteBehavior.Cascade)
                .OneToMany(user => user.UserPermissions, userPermission => userPermission.User, userPermission => userPermission.UserId, DeleteBehavior.Cascade);
        }
    }
}
