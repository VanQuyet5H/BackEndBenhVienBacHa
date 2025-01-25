using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs)
                .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs)
                .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);

            base.Configure(builder);
        }
    }
}
