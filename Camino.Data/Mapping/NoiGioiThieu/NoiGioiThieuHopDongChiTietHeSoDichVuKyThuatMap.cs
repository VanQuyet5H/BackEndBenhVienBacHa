using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDichVuKyThuatMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHeSoDichVuKyThuatTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuKyThuats)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuKyThuats)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuKyThuats)
                .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);

            base.Configure(builder);
        }
    }
}
