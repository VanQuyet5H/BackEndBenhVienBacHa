using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhLichSuTrangThaiMap: CaminoEntityTypeConfiguration<YeuCauKhamBenhLichSuTrangThai>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhLichSuTrangThai> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhLichSuTrangThaiTable);

            builder.HasOne(x => x.YeuCauKhamBenh)
                .WithMany(y => y.YeuCauKhamBenhLichSuTrangThais)
                .HasForeignKey(y => y.YeuCauKhamBenhId);

            base.Configure(builder);
        }
    }
}
