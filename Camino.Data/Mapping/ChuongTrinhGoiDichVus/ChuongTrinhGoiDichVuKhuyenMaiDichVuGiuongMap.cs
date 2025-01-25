using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongTable);

            builder
                .HasOne(sc => sc.ChuongTrinhGoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                .HasForeignKey(sc => sc.ChuongTrinhGoiDichVuId);

            builder
                .HasOne(sc => sc.DichVuGiuongBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                .HasForeignKey(sc => sc.DichVuGiuongBenhVienId);

            builder
                .HasOne(sc => sc.NhomGiaDichVuGiuongBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                .HasForeignKey(sc => sc.NhomGiaDichVuGiuongBenhVienId);

            base.Configure(builder);
        }
    }
}
