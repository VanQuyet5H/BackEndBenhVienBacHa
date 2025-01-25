using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVuDichVuKyThuats
{
    public class ChuongTrinhGoiDichVuDichVuKyThuatMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVuDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVuDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuDichVuKyThuatTable);

            builder
                .HasOne(sc => sc.ChuongTrinhGoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichVuKyThuats)
                .HasForeignKey(sc => sc.ChuongTrinhGoiDichVuId);

            builder
                .HasOne(sc => sc.DichVuKyThuatBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichVuKyThuats)
                .HasForeignKey(sc => sc.DichVuKyThuatBenhVienId);

            builder
                .HasOne(sc => sc.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(s => s.ChuongTrinhGoiDichVuDichVuKyThuats)
                .HasForeignKey(sc => sc.NhomGiaDichVuKyThuatBenhVienId);

            base.Configure(builder);
        }
    }
}
