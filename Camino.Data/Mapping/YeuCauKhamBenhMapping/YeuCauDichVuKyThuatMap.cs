using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuKyThuatMap : CaminoEntityTypeConfiguration<YeuCauDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuKyThuatTable);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                .WithMany(r => r.YeuCauDichVuKyThuatNhanVienChiDinhs)
                .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
             .WithMany(r => r.YeuCauDichVuKyThuatNhanVienDuyetBaoHiems)
             .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);

            builder.HasOne(rf => rf.NhanVienKetLuan)
              .WithMany(r => r.YeuCauDichVuKyThuatNhanVienKetLuans)
              .HasForeignKey(rf => rf.NhanVienKetLuanId);

            builder.HasOne(rf => rf.NhanVienThucHien)
             .WithMany(r => r.YeuCauDichVuKyThuatNhanVienThucHiens)
             .HasForeignKey(rf => rf.NhanVienThucHienId);



            builder.HasOne(rf => rf.NoiChiDinh)
               .WithMany(r => r.YeuCauDichVuKyThuatNoiChiDinhs)
               .HasForeignKey(rf => rf.NoiChiDinhId);

            builder.HasOne(rf => rf.NoiThucHien)
               .WithMany(r => r.YeuCauDichVuKyThuatNoiThucHiens)
               .HasForeignKey(rf => rf.NoiThucHienId);


            builder.HasOne(rf => rf.YeuCauKhamBenh)
                  .WithMany(r => r.YeuCauDichVuKyThuats)
                  .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                   .WithMany(r => r.YeuCauDichVuKyThuats)
                   .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                  .WithMany(r => r.YeuCauDichVuKyThuats)
                  .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.YeuCauDichVuKyThuatNhanVienHuyThanhToans)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);

            //builder.HasOne(rf => rf.FileKetQuaCanLamSang)
            //      .WithMany(r => r.YeuCauDichVuKyThuats)
            //      .HasForeignKey(rf => rf.FileKetQuaCanLamSangId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            builder.HasOne(rf => rf.TheoDoiSauPhauThuatThuThuat)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.TheoDoiSauPhauThuatThuThuatId);
            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.MayTraKetQua)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.MayTraKetQuaId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);

            builder.HasOne(rf => rf.GoiKhamSucKhoe)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.GoiKhamSucKhoeId);

            builder.HasOne(rf => rf.NhanVienHuyDichVu)
                .WithMany(r => r.YeuCauDichVuKyThuatNhanVienHuyDichVus)
                .HasForeignKey(rf => rf.NhanVienHuyDichVuId);


            builder.HasOne(rf => rf.NhanVienKetLuan2)
                .WithMany(r => r.YeuCauDichVuKyThuatNhanVienKetLuan2s)
                .HasForeignKey(rf => rf.NhanVienKetLuan2Id);

            builder.HasOne(rf => rf.NhanVienThucHien2)
                .WithMany(r => r.YeuCauDichVuKyThuatNhanVienThucHien2s)
                .HasForeignKey(rf => rf.NhanVienThucHien2Id);

            builder.HasOne(rf => rf.NhanVienHuyTrangThaiDaThucHien)
                .WithMany(r => r.YeuCauDichVuKyThuatNhanVienHuyTrangThaiDaThucHiens)
                .HasForeignKey(rf => rf.NhanVienHuyTrangThaiDaThucHienId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                .WithMany(r => r.YeuCauDichVuKyThuats)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatKhamSangLocTiemChungId);

            builder.HasOne(rf => rf.GoiKhamSucKhoeDichVuPhatSinh)
                .WithMany(r => r.YeuCauDichVuKyThuatKhamSucKhoeDichVuPhatSinhs)
                .HasForeignKey(rf => rf.GoiKhamSucKhoeDichVuPhatSinhId);

            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauDichVuKyThuats)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);


            base.Configure(builder);
        }
    }
}
