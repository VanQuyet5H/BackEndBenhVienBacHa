using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhBoPhanTonThuongMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhBoPhanTonThuong>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhBoPhanTonThuong> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhBoPhanTonThuongTable);

            builder.HasOne(x => x.YeuCauKhamBenh)
                .WithMany(y => y.YeuCauKhamBenhBoPhanTonThuongs)
                .HasForeignKey(y => y.YeuCauKhamBenhId);

            base.Configure(builder);
        }
    }
}
