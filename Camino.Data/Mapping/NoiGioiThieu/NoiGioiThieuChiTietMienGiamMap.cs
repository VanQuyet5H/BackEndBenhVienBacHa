using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuChiTietMienGiamMap : CaminoEntityTypeConfiguration<NoiGioiThieuChiTietMienGiam>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuChiTietMienGiam> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuChiTietMienGiamTable);

            builder.HasOne(rf => rf.NoiGioiThieu)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.NoiGioiThieuId);
            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);
            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);
            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);
            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);
            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.NoiGioiThieuChiTietMienGiams)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            base.Configure(builder);
        }
    }
}
