using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonThuocThanhToans
{
    public class DonThuocThanhToanChiTietTheoPhieuThuMap : CaminoEntityTypeConfiguration<DonThuocThanhToanChiTietTheoPhieuThu>
    {
        public override void Configure(EntityTypeBuilder<DonThuocThanhToanChiTietTheoPhieuThu> builder)
        {
            builder.ToTable(MappingDefaults.DonThuocThanhToanChiTietTheoPhieuThuTable);
            
            builder.HasOne(rf => rf.DuocPham)
                .WithMany()
                .HasForeignKey(rf => rf.DuocPhamId);
            builder.HasOne(rf => rf.DuongDung)
                .WithMany()
                .HasForeignKey(rf => rf.DuongDungId);
            builder.HasOne(rf => rf.DonViTinh)
                .WithMany()
                .HasForeignKey(rf => rf.DonViTinhId);
            builder.HasOne(rf => rf.HopDongThauDuocPham)
                .WithMany()
                .HasForeignKey(rf => rf.HopDongThauDuocPhamId);
            builder.HasOne(rf => rf.NhaThau)
                .WithMany()
                .HasForeignKey(rf => rf.NhaThauId);
            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                .WithMany()
                .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);
            builder.HasOne(rf => rf.BacSiKeDon)
                .WithMany()
                .HasForeignKey(rf => rf.BacSiKeDonId);

            base.Configure(builder);
        }
    }
}
