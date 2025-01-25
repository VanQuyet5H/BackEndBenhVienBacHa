using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class GoiKhamSucKhoeChungDichVuDichVuKyThuatMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeChungDichVuDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeChungDichVuDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeChungDichVuDichVuKyThuatTable);

            builder.HasOne(x => x.GoiKhamSucKhoeChung)
                .WithMany(y => y.GoiKhamSucKhoeChungDichVuDichVuKyThuats)
                .HasForeignKey(x => x.GoiKhamSucKhoeChungId);

            builder.HasOne(x => x.DichVuKyThuatBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeChungDichVuDichVuKyThuats)
                .HasForeignKey(x => x.DichVuKyThuatBenhVienId);

            builder.HasOne(x => x.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeChungDichVuDichVuKyThuats)
                .HasForeignKey(x => x.NhomGiaDichVuKyThuatBenhVienId);
            base.Configure(builder);
        }
    }
}
