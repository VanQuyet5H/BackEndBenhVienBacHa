using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.HopDongThauVatTuMapping
{
    public class HopDongThauVatTuMap : CaminoEntityTypeConfiguration<HopDongThauVatTu>
    {
        public override void Configure(EntityTypeBuilder<HopDongThauVatTu> builder)
        {
            builder.ToTable(MappingDefaults.HopDongThauVatTuTable);

            builder.HasOne(rf => rf.NhaThau)
                .WithMany(r => r.HopDongThauVatTus)
                .HasForeignKey(rf => rf.NhaThauId);

            base.Configure(builder);
        }
    }
}
