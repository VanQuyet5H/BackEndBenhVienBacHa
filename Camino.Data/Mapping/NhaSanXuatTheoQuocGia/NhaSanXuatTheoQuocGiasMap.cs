using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhaSanXuatTheoQuocGia
{
    public class NhaSanXuatTheoQuocGiasMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia> builder)
        {
            builder.ToTable(MappingDefaults.NhaSanXuatTheoQuocGiaTable);

            //builder.HasKey(sc => new { sc.NhaSanXuatId, sc.QuocGiaId });

            builder
                .HasOne<Core.Domain.Entities.NhaSanXuats.NhaSanXuat>(sc =>sc.NhaSanXuat)
                .WithMany(s => s.NhaSanXuatTheoQuocGias)
                .HasForeignKey(sc => sc.NhaSanXuatId);
           

            builder
                .HasOne<Core.Domain.Entities.QuocGias.QuocGia>(sc => sc.QuocGia)
                .WithMany(s => s.NhaSanXuatTheoQuocGias)
                .HasForeignKey(sc => sc.QuocGiaId);

            base.Configure(builder);
        }
    }
}
