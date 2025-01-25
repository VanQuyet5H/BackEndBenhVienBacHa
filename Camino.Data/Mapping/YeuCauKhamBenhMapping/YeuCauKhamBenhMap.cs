using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhMap : CaminoEntityTypeConfiguration<YeuCauKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhTable);

            builder.HasOne(rf => rf.NoiDangKy)
                  .WithMany(r => r.YeuCauKhamBenhNoiDangKys)
                  .HasForeignKey(rf => rf.NoiDangKyId);

            builder.HasOne(rf => rf.NoiThucHien)
                  .WithMany(r => r.YeuCauKhamBenhNoiThucHiens)
                  .HasForeignKey(rf => rf.NoiThucHienId);

            builder.HasOne(rf => rf.NoiChiDinh)
                  .WithMany(r => r.YeuCauKhamBenhNoiChiDinhs)
                  .HasForeignKey(rf => rf.NoiChiDinhId);

            builder.HasOne(rf => rf.NoiKetLuan)
                .WithMany(r => r.YeuCauKhamBenhNoiKetLuans)
                .HasForeignKey(rf => rf.NoiKetLuanId);

            builder.HasOne(rf => rf.YeuCauKhamBenhTruoc)
                .WithMany(r => r.InverseYeuCauKhamBenhTruocs)
                .HasForeignKey(rf => rf.YeuCauKhamBenhTruocId);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.YeuCauKhamBenhs)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.BacSiDangKy)
               .WithMany(r => r.YeuCauKhamBenhBacSiDangKys)
               .HasForeignKey(rf => rf.BacSiDangKyId);

            builder.HasOne(rf => rf.BacSiThucHien)
               .WithMany(r => r.YeuCauKhamBenhBacSiThucHiens)
               .HasForeignKey(rf => rf.BacSiThucHienId);

            builder.HasOne(rf => rf.BacSiKetLuan)
              .WithMany(r => r.YeuCauKhamBenhBacSiKetLuans)
              .HasForeignKey(rf => rf.BacSiKetLuanId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                .WithMany(r => r.YeuCauKhamBenhNhanVienChiDinhs)
                .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
               .WithMany(r => r.YeuCauKhamBenhNhanVienDuyetBaoHiems)
               .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);

            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.YeuCauKhamBenhNhanVienHuyThanhToans)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);

            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
              .WithMany(r => r.YeuCauKhamBenhs)
              .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);

            builder.HasOne(rf => rf.Icdchinh)
              .WithMany(r => r.YeuCauKhamBenhs)
              .HasForeignKey(rf => rf.IcdchinhId);

            builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(r => r.YeuCauKhamBenhs)
                .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);

            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                      .WithMany(r => r.YeuCauKhamBenhs)
                      .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            ///Update 02/06/2020
            builder.HasOne(rf => rf.BenhVienChuyenVien)
                     .WithMany(r => r.YeuCauKhamBenhs)
                     .HasForeignKey(rf => rf.BenhVienChuyenVienId);

            builder.HasOne(rf => rf.KhoaPhongNhapVien)
                     .WithMany(r => r.YeuCauKhamBenhs)
                     .HasForeignKey(rf => rf.KhoaPhongNhapVienId);

            //builder.Property(u => u.Ten).HasMaxLength(20);
            //builder.Property(u => u.TenVietTat).HasMaxLength(200);

            builder.HasOne(rf => rf.NhanVienHoTongChuyenVien)
             .WithMany(r => r.YeuCauKhamBenhNhanVienHoTongs)
             .HasForeignKey(rf => rf.NhanVienHoTongChuyenVienId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.YeuCauKhamBenhs)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);

            builder.HasOne(rf => rf.GoiKhamSucKhoe)
                .WithMany(r => r.YeuCauKhamBenhs)
                .HasForeignKey(rf => rf.GoiKhamSucKhoeId);

            builder.HasOne(rf => rf.NhanVienHuyDichVu)
                .WithMany(r => r.YeuCauKhamBenhNhanVienHuyDichVus)
                .HasForeignKey(rf => rf.NhanVienHuyDichVuId);

            builder.HasOne(rf => rf.GoiKhamSucKhoeDichVuPhatSinh)
                .WithMany(r => r.YeuCauKhamBenhKhamSucKhoeDichVuPhatSinhs)
                .HasForeignKey(rf => rf.GoiKhamSucKhoeDichVuPhatSinhId);

            builder.HasOne(rf => rf.NghiHuongBHXHNguoiIn)
               .WithMany(r => r.YeuCauKhamBenhNghiHuongBHXHNguoiIns)
               .HasForeignKey(rf => rf.NghiHuongBHXHNguoiInId);

            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauKhamBenhs)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
