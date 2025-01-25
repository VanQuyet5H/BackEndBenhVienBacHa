using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class GoiKhamSucKhoeDichVuDichVuKyThuatMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeDichVuDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeDichVuDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeDichVuDichVuKyThuatTable);

            builder.HasOne(x => x.GoiKhamSucKhoe)
                .WithMany(y => y.GoiKhamSucKhoeDichVuDichVuKyThuats)
                .HasForeignKey(x => x.GoiKhamSucKhoeId);

            builder.HasOne(x => x.DichVuKyThuatBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeDichVuDichVuKyThuats)
                .HasForeignKey(x => x.DichVuKyThuatBenhVienId);

            builder.HasOne(x => x.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeDichVuDichVuKyThuats)
                .HasForeignKey(x => x.NhomGiaDichVuKyThuatBenhVienId);
            base.Configure(builder);
        }
    }
}
