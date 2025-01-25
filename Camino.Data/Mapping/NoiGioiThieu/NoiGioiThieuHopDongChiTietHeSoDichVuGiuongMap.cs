using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDichVuGiuongMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHeSoDichVuGiuongTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuGiuongs)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuGiuongs)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuGiuongs)
                .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);

            base.Configure(builder);
        }
    }
}
