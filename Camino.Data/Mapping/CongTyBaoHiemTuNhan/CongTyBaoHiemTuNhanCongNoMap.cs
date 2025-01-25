using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CongTyBaoHiemTuNhan
{
    public class CongTyBaoHiemTuNhanCongNoMap : CaminoEntityTypeConfiguration<CongTyBaoHiemTuNhanCongNo>
    {
        public override void Configure(EntityTypeBuilder<CongTyBaoHiemTuNhanCongNo> builder)
        {
            builder.ToTable(MappingDefaults.CongTyBaoHiemTuNhanCongNoTable);

            builder.HasOne(rf => rf.CongTyBaoHiemTuNhan)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.CongTyBaoHiemTuNhanId);

            builder.HasOne(rf => rf.TaiKhoanBenhNhanThu)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.TaiKhoanBenhNhanThuId);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.YeuCauDuocPhamBenhVien)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauDuocPhamBenhVienId);

            builder.HasOne(rf => rf.YeuCauVatTuBenhVien)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauVatTuBenhVienId);

            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVien)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.DonThuocThanhToanChiTiet)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.DonThuocThanhToanChiTietId);

            builder.HasOne(rf => rf.DonVTYTThanhToanChiTiet)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.DonVTYTThanhToanChiTietId);

            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            builder.HasOne(rf => rf.YeuCauTruyenMau)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauTruyenMauId);

            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBenhVien)
                .WithMany(r => r.CongTyBaoHiemTuNhanCongNos)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId);

            base.Configure(builder);
        }
    }
}
