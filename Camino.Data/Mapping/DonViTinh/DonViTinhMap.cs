using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DonViTinh
{
    public class DonViTinhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DonViTinhs.DonViTinh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DonViTinhs.DonViTinh> builder)
        {
            builder.ToTable(MappingDefaults.DonViTinhTable);
            base.Configure(builder);
        }
    }
}
