using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuXetNghiem
{
    public class DichVuXetNghiemKetNoiChiSoMap : CaminoEntityTypeConfiguration<DichVuXetNghiemKetNoiChiSo>
    {
        public override void Configure(EntityTypeBuilder<DichVuXetNghiemKetNoiChiSo> builder)
        {
            builder.ToTable(MappingDefaults.DichVuXetNghiemKetNoiChiSoTable);

            builder.HasOne(rf => rf.DichVuXetNghiem)
                .WithMany(r => r.DichVuXetNghiemKetNoiChiSos)
                .HasForeignKey(rf => rf.DichVuXetNghiemId);
            builder.HasOne(rf => rf.MauMayXetNghiem)
                .WithMany(r => r.DichVuXetNghiemKetNoiChiSos)
                .HasForeignKey(rf => rf.MauMayXetNghiemId);

            base.Configure(builder);
        }
    }
}
