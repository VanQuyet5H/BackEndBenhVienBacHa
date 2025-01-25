using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class GoiKhamSucKhoeDichVuKhamBenhMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeDichVuKhamBenhTable);

            builder.HasOne(x => x.GoiKhamSucKhoe)
                .WithMany(y => y.GoiKhamSucKhoeDichVuKhamBenhs)
                .HasForeignKey(x => x.GoiKhamSucKhoeId);

            builder.HasOne(x => x.DichVuKhamBenhBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeDichVuKhamBenhs)
                .HasForeignKey(x => x.DichVuKhamBenhBenhVienId);

            builder.HasOne(x => x.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeDichVuKhamBenhs)
                .HasForeignKey(x => x.NhomGiaDichVuKhamBenhBenhVienId);
            base.Configure(builder);
        }
    }
}
