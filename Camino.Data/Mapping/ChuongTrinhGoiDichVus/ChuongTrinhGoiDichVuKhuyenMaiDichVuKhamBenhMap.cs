using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhTable);

            builder
                .HasOne(sc => sc.ChuongTrinhGoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .HasForeignKey(sc => sc.ChuongTrinhGoiDichVuId);

            builder
                .HasOne(sc => sc.DichVuKhamBenhBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .HasForeignKey(sc => sc.DichVuKhamBenhBenhVienId);

            builder
                .HasOne(sc => sc.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .HasForeignKey(sc => sc.NhomGiaDichVuKhamBenhBenhVienId);

            base.Configure(builder);
        }
    }
}
