﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Leveranciers;



namespace Rise.Persistence.Leveranciers 
{
    internal class LeverancierConfiguration : EntityConfiguration<Leverancier>
    {
        public override void Configure(EntityTypeBuilder<Leverancier> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Address)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}