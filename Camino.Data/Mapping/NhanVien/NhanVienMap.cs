using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NhanVien
{
    public class NhanVienMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.NhanViens.NhanVien>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.NhanViens.NhanVien> builder)
        {
            builder.ToTable(MappingDefaults.NhanVien);

            builder.HasOne(rf => rf.User)
              .WithOne(r => r.NhanVien).
              HasForeignKey<Core.Domain.Entities.NhanViens.NhanVien>(c => c.Id);

            builder.HasOne(rf => rf.ChucDanh)
               .WithMany(r => r.NhanViens)
               .HasForeignKey(rf => rf.ChucDanhId);

            builder.HasOne(rf => rf.HocHamHocVi)
              .WithMany(r => r.NhanViens)
              .HasForeignKey(rf => rf.HocHamHocViId);

            builder.HasOne(rf => rf.VanBangChuyenMon)
             .WithMany(r => r.NhanViens)
             .HasForeignKey(rf => rf.VanBangChuyenMonId);

            builder.HasOne(rf => rf.PhamViHanhNghe)
             .WithMany(r => r.NhanViens)
             .HasForeignKey(rf => rf.PhamViHanhNgheId);

            builder.HasMany(rf => rf.ToaThuocMaus)
             .WithOne(r => r.BacSiKeToa)
             .HasForeignKey(rf => rf.BacSiKeToaId).IsRequired();

            builder.HasMany(rf => rf.NguoiGioiThieus)
             .WithOne(r => r.NhanVien)
             .HasForeignKey(rf => rf.NhanVienQuanLyId).IsRequired();

            builder.HasMany(rf => rf.NoiGioiThieus)
           .WithOne(r => r.NhanVienQuanLy)
           .HasForeignKey(rf => rf.NhanVienQuanLyId);

            builder.HasMany(rf => rf.KhoaPhongNhanViens)
           .WithOne(r => r.NhanVien)
           .HasForeignKey(rf => rf.NhanVienId);

            builder.HasMany(rf => rf.NhanVienRoles)
            .WithOne(r => r.NhanVien)
            .HasForeignKey(rf => rf.NhanVienId);

            builder.HasMany(rf => rf.LichPhanCongNgoaiTrus)
           .WithOne(r => r.NhanVien)
           .HasForeignKey(rf => rf.NhanVienId);

            builder.HasMany(rf => rf.LichSuHoatDongNhanViens)
              .WithOne(r => r.NhanVien)
              .HasForeignKey(rf => rf.NhanVienId);

            builder.HasMany(rf => rf.HoatDongNhanViens)
           .WithOne(r => r.NhanVien)
           .HasForeignKey(rf => rf.NhanVienId);

            builder.HasMany(rf => rf.KetQuaNhomXetNghiems)
           .WithOne(r => r.NhanVienKetLuan)
           .HasForeignKey(rf => rf.NhanVienKetLuanId);

            base.Configure(builder);
        }
    }
}
