using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuGiuongBenhVienChiPhiBenhVienMap : CaminoEntityTypeConfiguration<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuGiuongBenhVienChiPhiBenhVienTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.GiuongBenh)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.GiuongBenhId);
            builder.HasOne(rf => rf.PhongBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.PhongBenhVienId);
            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);
            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
