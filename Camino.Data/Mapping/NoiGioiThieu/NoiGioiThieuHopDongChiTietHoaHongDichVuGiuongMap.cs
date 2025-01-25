using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongs)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongs)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongs)
                .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);

            base.Configure(builder);
        }
    }
}
