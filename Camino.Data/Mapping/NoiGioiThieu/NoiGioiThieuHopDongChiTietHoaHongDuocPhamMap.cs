using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDuocPhamMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHoaHongDuocPham>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHoaHongDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHoaHongDuocPhamTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDuocPhams)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDuocPhams)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            base.Configure(builder);
        }
    }
}
