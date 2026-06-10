using Template.Database.Configuration;
using Template.Database.Domain.Entities.Identity;

namespace Template.Database.Domain.Configurations.Identity
{
    public class UserDetailsConfiguration : EntityConfiguration<UserDetails>
    {
        protected override void Configure(EntityConfigurationBuilder<UserDetails> builder)
        {
            builder
                .Table("UserDetails", "identity")
                .Key(userDetails => userDetails.Id)
                .Property(userDetails => userDetails.Id, property => property.Column("UserDetailsId").GeneratedOnAdd())
                .Property(userDetails => userDetails.UserId, property => property.Column("UserId").Required())
                .Property(userDetails => userDetails.FirstName, property => property.Column("FirstName").Required().MaxLength(100))
                .Property(userDetails => userDetails.LastName, property => property.Column("LastName").Required().MaxLength(100))
                .Property(userDetails => userDetails.PhoneNumber, property => property.Column("PhoneNumber").MaxLength(20))
                .Index(userDetails => userDetails.UserId, isUnique: true, databaseName: "IX_UserDetails_UserId");
        }
    }
}
