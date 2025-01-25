using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
     public class GoiKhamSucKhoeNoiThucHienMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeNoiThucHien>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeNoiThucHien> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeNoiThucHienTable);

            builder.HasOne(x => x.GoiKhamSucKhoeDichVuKhamBenh)
                .WithMany(y => y.GoiKhamSucKhoeNoiThucHiens)
                .HasForeignKey(x => x.GoiKhamSucKhoeDichVuKhamBenhId);

            builder.HasOne(x => x.GoiKhamSucKhoeDichVuDichVuKyThuat)
                .WithMany(y => y.GoiKhamSucKhoeNoiThucHiens)
                .HasForeignKey(x => x.GoiKhamSucKhoeDichVuDichVuKyThuatId);

            builder.HasOne(x => x.PhongBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeNoiThucHiens)
                .HasForeignKey(x => x.PhongBenhVienId);
            base.Configure(builder);
        }
    }
}
