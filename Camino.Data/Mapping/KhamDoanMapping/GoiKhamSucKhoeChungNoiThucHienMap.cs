using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
     public class GoiKhamSucKhoeChungNoiThucHienMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeChungNoiThucHien>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeChungNoiThucHien> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeChungNoiThucHienTable);

            builder.HasOne(x => x.GoiKhamSucKhoeChungDichVuKhamBenh)
                .WithMany(y => y.GoiKhamSucKhoeChungNoiThucHiens)
                .HasForeignKey(x => x.GoiKhamSucKhoeChungDichVuKhamBenhId);

            builder.HasOne(x => x.GoiKhamSucKhoeChungDichVuDichVuKyThuat)
                .WithMany(y => y.GoiKhamSucKhoeChungNoiThucHiens)
                .HasForeignKey(x => x.GoiKhamSucKhoeChungDichVuDichVuKyThuatId);

            builder.HasOne(x => x.PhongBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeChungNoiThucHiens)
                .HasForeignKey(x => x.PhongBenhVienId);
            base.Configure(builder);
        }
    }
}
