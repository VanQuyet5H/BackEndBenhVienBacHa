using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatTable);

            builder
                .HasOne(sc => sc.ChuongTrinhGoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .HasForeignKey(sc => sc.ChuongTrinhGoiDichVuId);

            builder
                .HasOne(sc => sc.DichVuKyThuatBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .HasForeignKey(sc => sc.DichVuKyThuatBenhVienId);

            builder
                .HasOne(sc => sc.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .HasForeignKey(sc => sc.NhomGiaDichVuKyThuatBenhVienId);

            base.Configure(builder);
        }
    }
}
