using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.HopDongThauVatTuMapping
{
    public class HopDongThauVatTuChiTietMap : CaminoEntityTypeConfiguration<HopDongThauVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<HopDongThauVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.HopDongThauVatTuChiTietTable);

            builder.HasOne(rf => rf.HopDongThauVatTu)
                .WithMany(r => r.HopDongThauVatTuChiTiets)
                .HasForeignKey(rf => rf.HopDongThauVatTuId);

            builder.HasOne(rf => rf.VatTu)
                .WithMany(r => r.HopDongThauVatTuChiTiets)
                .HasForeignKey(rf => rf.VatTuId);

            base.Configure(builder);
        }
    }
}
