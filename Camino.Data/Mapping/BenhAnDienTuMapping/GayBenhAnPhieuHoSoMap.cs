using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhAnDienTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhAnDienTuMapping
{
    public class GayBenhAnPhieuHoSoMap : CaminoEntityTypeConfiguration<GayBenhAnPhieuHoSo>
    {
        public override void Configure(EntityTypeBuilder<GayBenhAnPhieuHoSo> builder)
        {
            builder.ToTable(MappingDefaults.GayBenhAnPhieuHoSoTable);

            builder.HasOne(x => x.GayBenhAn)
                .WithMany(y => y.GayBenhAnPhieuHoSos)
                .HasForeignKey(x => x.GayBenhAnId);

            base.Configure(builder);
        }
    }
}
