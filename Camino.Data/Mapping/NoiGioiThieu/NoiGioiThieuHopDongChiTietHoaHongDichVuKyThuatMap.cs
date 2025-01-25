using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuatMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuatTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats)
                .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);

            base.Configure(builder);
        }
    }
}
