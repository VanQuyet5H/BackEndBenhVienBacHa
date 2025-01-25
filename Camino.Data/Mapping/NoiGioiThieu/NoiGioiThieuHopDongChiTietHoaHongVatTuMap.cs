using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongVatTuMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHoaHongVatTu>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHoaHongVatTu> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHoaHongVatTuTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongVatTus)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongVatTus)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            base.Configure(builder);
        }
    }
}
