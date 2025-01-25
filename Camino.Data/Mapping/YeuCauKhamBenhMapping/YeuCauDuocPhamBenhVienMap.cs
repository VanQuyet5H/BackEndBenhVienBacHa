using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDuocPhamBenhVienMap : CaminoEntityTypeConfiguration<YeuCauDuocPhamBenhVien>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDuocPhamBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDuocPhamBenhVienTable);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                    .WithMany(r => r.YeuCauDuocPhamBenhViens)
                    .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            //builder.HasOne(rf => rf.GoiDichVu)
            //          .WithMany(r => r.YeuCauDuocPhamBenhViens)
            //          .HasForeignKey(rf => rf.GoiDichVuId);

            builder.HasOne(rf => rf.NhanVienCapThuoc)
                     .WithMany(r => r.YeuCauDuocPhamBenhVienNhanVienCapThuocs)
                     .HasForeignKey(rf => rf.NhanVienCapThuocId);


            builder.HasOne(rf => rf.NhanVienChiDinh)
                     .WithMany(r => r.YeuCauDuocPhamBenhVienNhanVienChiDinhs)
                     .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                     .WithMany(r => r.YeuCauDuocPhamBenhVienNhanVienDuyetBaoHiems)
                     .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);

            //builder.HasOne(rf => rf.NhanVienThanhToan)
            //         .WithMany(r => r.YeuCauDuocPhamBenhVienNhanVienThanhToans)
            //         .HasForeignKey(rf => rf.NhanVienThanhToanId);

            builder.HasOne(rf => rf.NoiCapThuoc)
                       .WithMany(r => r.YeuCauDuocPhamBenhVienNoiCapThuocs)
                       .HasForeignKey(rf => rf.NoiCapThuocId);

            builder.HasOne(rf => rf.NoiChiDinh)
                   .WithMany(r => r.YeuCauDuocPhamBenhVienNoiChiDinhs)
                   .HasForeignKey(rf => rf.NoiChiDinhId);

            //builder.HasOne(rf => rf.NoiThanhToan)
            //        .WithMany(r => r.YeuCauDuocPhamBenhVienNoiThanhToans)
            //        .HasForeignKey(rf => rf.NoiThanhToanId);


            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                    .WithMany(r => r.YeuCauDuocPhamBenhViens)
                    .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                      .WithMany(r => r.YeuCauDuocPhamBenhViens)
                      .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                      .WithMany(r => r.YeuCauDuocPhamBenhViens)
                      .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.NhaThau)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.NhaThauId);

            builder.HasOne(rf => rf.HopDongThauDuocPham)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.HopDongThauDuocPhamId);

            builder.HasOne(rf => rf.DonViTinh)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.DonViTinhId);

            builder.HasOne(rf => rf.DuongDung)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.DuongDungId);

            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                    .WithMany(r => r.YeuCauDuocPhamBenhViens)
                    .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.YeuCauDuocPhamBenhVienNhanVienHuyThanhToans)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);

            builder.HasOne(rf => rf.XuatKhoDuocPhamChiTiet)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.XuatKhoDuocPhamChiTietId);

            builder.HasOne(rf => rf.YeuCauLinhDuocPham)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.YeuCauLinhDuocPhamId);

            //builder.HasOne(rf => rf.XuatKhoDuocPhamChiTietViTri)
            //   .WithMany(r => r.YeuCauDuocPhamBenhViens)
            //   .HasForeignKey(rf => rf.XuatKhoDuocPhamChiTietViTriId);
            builder.HasOne(rf => rf.KhoLinh)
              .WithMany(r => r.YeuCauDuocPhamBenhViens)
              .HasForeignKey(rf => rf.KhoLinhId);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.NoiTruChiDinhDuocPham)
              .WithMany(r => r.YeuCauDuocPhamBenhViens)
              .HasForeignKey(rf => rf.NoiTruChiDinhDuocPhamId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.YeuCauDuocPhamBenhViens)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);

            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauDuocPhamBenhViens)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
