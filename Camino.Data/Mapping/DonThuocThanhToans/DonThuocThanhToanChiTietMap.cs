using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonThuocThanhToans
{
   
    public class DonThuocThanhToanChiTietMap : CaminoEntityTypeConfiguration<DonThuocThanhToanChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DonThuocThanhToanChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DonThuocThanhToanChiTietTable);

            builder.HasOne(rf => rf.DonThuocThanhToan)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.DonThuocThanhToanId);
            builder.HasOne(rf => rf.YeuCauKhamBenhDonThuocChiTiet)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.YeuCauKhamBenhDonThuocChiTietId);
            builder.HasOne(rf => rf.DuocPham)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.DuocPhamId);
            builder.HasOne(rf => rf.XuatKhoDuocPhamChiTietViTri)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.XuatKhoDuocPhamChiTietViTriId);
            builder.HasOne(rf => rf.DuongDung)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.DuongDungId);
            builder.HasOne(rf => rf.DonViTinh)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.DonViTinhId);
            builder.HasOne(rf => rf.HopDongThauDuocPham)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.HopDongThauDuocPhamId);
            builder.HasOne(rf => rf.NhaThau)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.NhaThauId);
            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);
            //update 15/06/2021
            builder.HasOne(rf => rf.NoiTruDonThuocChiTiet)
                .WithMany(r => r.DonThuocThanhToanChiTiets)
                .HasForeignKey(rf => rf.NoiTruDonThuocChiTietId);

            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.DonThuocThanhToanChiTiets)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
