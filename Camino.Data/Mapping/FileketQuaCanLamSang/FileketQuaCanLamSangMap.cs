using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.FileketQuaCanLamSang
{
   public  class FileketQuaCanLamSangMap : CaminoEntityTypeConfiguration<FileKetQuaCanLamSang>
    {
        public override void Configure(EntityTypeBuilder<FileKetQuaCanLamSang> builder)
        {
            builder.ToTable(MappingDefaults.FileKetQuaCanLamSangTable);

            builder.HasOne(rf => rf.KetQuaNhomXetNghiem)
                .WithMany(r => r.FileKetQuaCanLamSangs)
                .HasForeignKey(rf => rf.KetQuaNhomXetNghiemId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.FileKetQuaCanLamSangs)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            base.Configure(builder);
        }
    }
}
