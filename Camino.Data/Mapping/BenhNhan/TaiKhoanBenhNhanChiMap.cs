using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class TaiKhoanBenhNhanChiMap : CaminoEntityTypeConfiguration<TaiKhoanBenhNhanChi>
    {
        public override void Configure(EntityTypeBuilder<TaiKhoanBenhNhanChi> builder)
        {
            builder.ToTable(MappingDefaults.TaiKhoanBenhNhanChiTable);

            builder.HasOne(rf => rf.TaiKhoanBenhNhan)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.TaiKhoanBenhNhanId);
            builder.HasOne(rf => rf.TaiKhoanBenhNhanThu)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.TaiKhoanBenhNhanThuId);
            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);
            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);
            builder.HasOne(rf => rf.YeuCauDuocPhamBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauDuocPhamBenhVienId);
            builder.HasOne(rf => rf.YeuCauVatTuBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauVatTuBenhVienId);
            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienId);
            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauGoiDichVuId);
            builder.HasOne(rf => rf.DonThuocThanhToanChiTiet)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.DonThuocThanhToanChiTietId);
            builder.HasOne(rf => rf.DonVTYTThanhToanChiTiet)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.DonVTYTThanhToanChiTietId);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.NhanVienThucHienId);
            builder.HasOne(rf => rf.NhanVienHuy)
                .WithMany(r => r.TaiKhoanBenhNhanChiHuys)
                .HasForeignKey(rf => rf.NhanVienHuyId);
            builder.HasOne(rf => rf.NhanVienThuHoi)
                .WithMany(r => r.TaiKhoanBenhNhanChiThuHois)
                .HasForeignKey(rf => rf.NhanVienThuHoiId);
            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.NoiThucHienId);
            builder.HasOne(rf => rf.NoiHuy)
                .WithMany(r => r.TaiKhoanBenhNhanChiNoiHuys)
                .HasForeignKey(rf => rf.NoiHuyId);
            builder.HasOne(rf => rf.YeuCauTruyenMau)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauTruyenMauId);
            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId);
            builder.HasOne(rf => rf.PhieuThanhToanChiPhi)
                .WithMany(r => r.TaiKhoanBenhNhanChis)
                .HasForeignKey(rf => rf.PhieuThanhToanChiPhiId);
            builder.HasOne(rf => rf.DonThuocThanhToanChiTietTheoPhieuThu)
                .WithMany()
                .HasForeignKey(rf => rf.DonThuocThanhToanChiTietTheoPhieuThuId);
            builder.HasOne(rf => rf.DonVTYTThanhToanChiTietTheoPhieuThu)
                .WithMany()
                .HasForeignKey(rf => rf.DonVTYTThanhToanChiTietTheoPhieuThuId);

            base.Configure(builder);
        }
    }
}
