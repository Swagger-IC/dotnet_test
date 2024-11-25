﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Products;

namespace Rise.Persistence.Products;

/// <summary>
/// Specific configuration for <see cref="Product"/>.
/// </summary>
internal class ProductConfiguration : EntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(250) 
            .IsRequired(); 

        builder.Property(x => x.Location)
            .HasMaxLength(250) 
            .IsRequired(); 

        builder.Property(x => x.Description)
            .HasMaxLength(500) 
            .IsRequired(); 

        builder.Property(x => x.Reusable)
            .IsRequired();

        builder.Property(x => x.Barcode).HasMaxLength(48).IsRequired();

        builder.Property(x => x.Keywords)
            .IsRequired();
    }
}