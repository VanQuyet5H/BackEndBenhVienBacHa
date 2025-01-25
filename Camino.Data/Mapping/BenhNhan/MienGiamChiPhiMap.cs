using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class MienGiamChiPhiMap : CaminoEntityTypeConfiguration<MienGiamChiPhi>
    {
        public override void Configure(EntityTypeBuilder<MienGiamChiPhi> builder)
        {
            builder.ToTable(MappingDefaults.MienGiamChiPhiTable);

            builder.HasOne(rf => rf.TaiKhoanBenhNhanThu)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.TaiKhoanBenhNhanThuId);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.TheVoucher)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.TheVoucherId);

            builder.HasOne(rf => rf.DoiTuongUuDai)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.DoiTuongUuDaiId);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.YeuCauDuocPhamBenhVien)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauDuocPhamBenhVienId);

            builder.HasOne(rf => rf.YeuCauVatTuBenhVien)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauVatTuBenhVienId);

            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVien)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            builder.HasOne(rf => rf.DonThuocThanhToanChiTiet)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.DonThuocThanhToanChiTietId);

            builder.HasOne(rf => rf.DonVTYTThanhToanChiTiet)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.DonVTYTThanhToanChiTietId);

            builder.HasOne(rf => rf.YeuCauTruyenMau)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauTruyenMauId);

            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBenhVien)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId);

            builder.HasOne(rf => rf.YeuCauGoiDichVuKhuyenMai)
                .WithMany(r => r.MienGiamChiPhiKhuyenMais)
                .HasForeignKey(rf => rf.YeuCauGoiDichVuKhuyenMaiId);

            builder.HasOne(rf => rf.NoiGioiThieu)
                .WithMany(r => r.MienGiamChiPhis)
                .HasForeignKey(rf => rf.NoiGioiThieuId);

            base.Configure(builder);
        }
    }
}
