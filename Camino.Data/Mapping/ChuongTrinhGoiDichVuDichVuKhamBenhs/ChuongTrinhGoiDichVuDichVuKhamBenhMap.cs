using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVuDichVuKhamBenhs
{
    public class ChuongTrinhGoiDichVuDichVuKhamBenhMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVuDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVuDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuDichVuKhamBenhTable);

            builder
                .HasOne(sc => sc.ChuongTrinhGoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichKhamBenhs)
                .HasForeignKey(sc => sc.ChuongTrinhGoiDichVuId);

            builder
                .HasOne(sc => sc.DichVuKhamBenhBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichKhamBenhs)
                .HasForeignKey(sc => sc.DichVuKhamBenhBenhVienId);

            builder
                .HasOne(sc => sc.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichKhamBenhs)
                .HasForeignKey(sc => sc.NhomGiaDichVuKhamBenhBenhVienId);

            base.Configure(builder);
        }
    }
}
