using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuGiuongBenhVienMap : CaminoEntityTypeConfiguration<YeuCauDichVuGiuongBenhVien>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuGiuongBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuGiuongBenhVienTable);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                   .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
                   .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                  .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
                  .HasForeignKey(rf => rf.YeuCauTiepNhanId)
                  .IsRequired();

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                 .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
                 .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
               .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
               .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
              .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
              .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);

            //builder.HasOne(rf => rf.GoiDichVu)
            //  .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
            //  .HasForeignKey(rf => rf.GoiDichVuId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
              .WithMany(r => r.YeuCauDichVuGiuongBenhVienNhanVienChiDinhs)
              .HasForeignKey(rf => rf.NhanVienChiDinhId);

            //builder.HasOne(rf => rf.NhanVienThanhToan)
            // .WithMany(r => r.YeuCauDichVuGiuongBenhVienNhanVienThanhToans)
            // .HasForeignKey(rf => rf.NhanVienThanhToanId);

            //builder.HasOne(rf => rf.NhanVienThanhToan)
            //  .WithMany(r => r.YeuCauDichVuGiuongBenhVienNhanVienThanhToans)
            //  .HasForeignKey(rf => rf.NhanVienThanhToanId);

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
             .WithMany(r => r.YeuCauDichVuGiuongBenhVienNhanVienDuyetBaoHiems)
             .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);

            builder.HasOne(rf => rf.NoiChiDinh)
               .WithMany(r => r.YeuCauDichVuGiuongBenhVienNoiChiDinhs)
               .HasForeignKey(rf => rf.NoiChiDinhId);

            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.NoiThucHienId);

            //builder.HasOne(rf => rf.NoiThanhToan)
            //   .WithMany(r => r.YeuCauDichVuGiuongBenhVienNoiThanhToans)
            //   .HasForeignKey(rf => rf.NoiThanhToanId);

            builder.HasOne(rf => rf.YeuCauGoiDichVu)
               .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
               .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienNhanVienHuyThanhToans)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);

            builder.HasOne(rf => rf.GiuongBenh)
                .WithMany(r => r.YeuCauDichVuGiuongBenhViens)
                .HasForeignKey(rf => rf.GiuongBenhId);

            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauDichVuGiuongBenhViens)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
