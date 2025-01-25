using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVuDichVuGiuongs
{
    public class ChuongTrinhGoiDichVuDichVuGiuongMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVuDichVuGiuong>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVuDichVuGiuong> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuDichVuGiuongTable);

            builder
                .HasOne(sc => sc.ChuongTrinhGoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichVuGiuongs)
                .HasForeignKey(sc => sc.ChuongTrinhGoiDichVuId);

            builder
                .HasOne(sc => sc.DichVuGiuongBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichVuGiuongs)
                .HasForeignKey(sc => sc.DichVuGiuongBenhVienId);

            builder
                .HasOne(sc => sc.NhomGiaDichVuGiuongBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichVuGiuongs)
                .HasForeignKey(sc => sc.NhomGiaDichVuGiuongBenhVienId);

            base.Configure(builder);
        }
    }
}
