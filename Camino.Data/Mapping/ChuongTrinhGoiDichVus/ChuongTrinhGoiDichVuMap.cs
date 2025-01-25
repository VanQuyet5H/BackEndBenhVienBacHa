using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVuMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVu>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVu> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuTable);

            builder
                .HasOne(sc => sc.GoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVus)
                .HasForeignKey(sc => sc.GoiDichVuId);

            builder
                .HasOne(sc => sc.CongTyBaoHiemTuNhan)
                .WithMany(s => s.ChuongTrinhGoiDichVus)
                .HasForeignKey(sc => sc.CongTyBaoHiemTuNhanId);

            builder
                .HasOne(sc => sc.BenhNhan)
                .WithMany(s => s.ChuongTrinhGoiDichVus)
                .HasForeignKey(sc => sc.BenhNhanId);

            builder
            .HasOne(sc => sc.LoaiGoiDichVu)
            .WithMany(s => s.ChuongTrinhGoiDichVus)
            .HasForeignKey(sc => sc.LoaiGoiDichVuId);

            base.Configure(builder);
        }
    }
}
