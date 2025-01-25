using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDuocPhamMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHeSoDuocPham>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHeSoDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHeSoDuocPhamTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDuocPhams)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDuocPhams)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            base.Configure(builder);
        }
    }
}
