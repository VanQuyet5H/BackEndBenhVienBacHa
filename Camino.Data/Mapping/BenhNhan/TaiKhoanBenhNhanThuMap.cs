using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class TaiKhoanBenhNhanThuMap : CaminoEntityTypeConfiguration<TaiKhoanBenhNhanThu>
    {
        public override void Configure(EntityTypeBuilder<TaiKhoanBenhNhanThu> builder)
        {
            builder.ToTable(MappingDefaults.TaiKhoanBenhNhanThuTable);

            builder.HasOne(rf => rf.TaiKhoanBenhNhan)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.TaiKhoanBenhNhanId);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.HoanTraYeuCauKhamBenh)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauKhamBenhId);
            builder.HasOne(rf => rf.HoanTraYeuCauDichVuKyThuat)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauDichVuKyThuatId);
            builder.HasOne(rf => rf.HoanTraYeuCauDuocPhamBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauDuocPhamBenhVienId);
            builder.HasOne(rf => rf.HoanTraYeuCauVatTuBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauVatTuBenhVienId);
            builder.HasOne(rf => rf.HoanTraYeuCauDichVuGiuongBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauDichVuGiuongBenhVienId);
            builder.HasOne(rf => rf.HoanTraYeuCauGoiDichVu)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauGoiDichVuId);
            builder.HasOne(rf => rf.HoanTraDonThuocThanhToan)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraDonThuocThanhToanId);
            builder.HasOne(rf => rf.HoanTraDonVTYTThanhToan)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraDonVTYTThanhToanId);
            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.NhanVienThucHienId);
            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.NoiThucHienId);
            builder.HasOne(rf => rf.TaiKhoanBenhNhanHuyDichVu)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.TaiKhoanBenhNhanHuyDichVuId);
            builder.HasOne(rf => rf.HoanTraYeuCauDichVuGiuongBenhVienChiPhiBenhVien)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauDichVuGiuongBenhVienChiPhiBenhVienId);
            builder.HasOne(rf => rf.HoanTraYeuCauTruyenMau)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.HoanTraYeuCauTruyenMauId);

            builder.HasOne(rf => rf.NhanVienHuy)
                .WithMany(r => r.TaiKhoanBenhNhanThuNhanVienHuys)
                .HasForeignKey(rf => rf.NhanVienHuyId);
            builder.HasOne(rf => rf.NhanVienThuHoi)
                .WithMany(r => r.TaiKhoanBenhNhanThuNhanVienThuHois)
                .HasForeignKey(rf => rf.NhanVienThuHoiId);
            builder.HasOne(rf => rf.NoiHuy)
                .WithMany(r => r.TaiKhoanBenhNhanThuNoiHuys)
                .HasForeignKey(rf => rf.NoiHuyId);
            builder.HasOne(rf => rf.PhieuHoanUng)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.PhieuHoanUngId);
            builder.HasOne(rf => rf.ThuNoPhieuThu)
                .WithMany(r => r.TaiKhoanBenhNhanThus)
                .HasForeignKey(rf => rf.ThuNoPhieuThuId);

            base.Configure(builder);
        }
    }
}
