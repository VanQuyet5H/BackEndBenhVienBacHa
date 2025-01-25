using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauTiepNhanLichSuChuyenDoiTuongs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping
{
    public class YeuCauTiepNhanLichSuChuyenDoiTuongs : CaminoEntityTypeConfiguration<YeuCauTiepNhanLichSuChuyenDoiTuong>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTiepNhanLichSuChuyenDoiTuong> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTiepNhanLichSuChuyenDoiTuongTable);

            builder.HasOne(x => x.YeuCauTiepNhan)
                .WithMany(y => y.YeuCauTiepNhanLichSuChuyenDoiTuongs)
                .HasForeignKey(y => y.YeuCauTiepNhanId);
            builder.HasOne(x => x.GiayMienCungChiTra)
                .WithMany(y => y.YeuCauTiepNhanLichSuChuyenDoiTuongs)
                .HasForeignKey(y => y.GiayMienCungChiTraId);

            base.Configure(builder);
        }
    }
}
