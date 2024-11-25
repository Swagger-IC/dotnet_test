using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Users
{
    internal class UserConfiguration : EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Naam)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Voornaam)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}
