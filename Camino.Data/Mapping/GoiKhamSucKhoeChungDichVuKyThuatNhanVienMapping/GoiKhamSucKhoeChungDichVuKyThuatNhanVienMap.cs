using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.GoiKhamSucKhoeChungDichVuKyThuatNhanVienMapping
{
    public class GoiKhamSucKhoeChungDichVuKyThuatNhanVienMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeChungDichVuKyThuatNhanVien>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeChungDichVuKyThuatNhanVienTable);

            builder.HasOne(rf => rf.GoiKhamSucKhoeDichVuDichVuKyThuat)
              .WithMany(r => r.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
              .HasForeignKey(rf => rf.GoiKhamSucKhoeDichVuDichVuKyThuatId);

            builder.HasOne(rf => rf.GoiKhamSucKhoe)
             .WithMany(r => r.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
             .HasForeignKey(rf => rf.GoiKhamSucKhoeId);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
             .WithMany(r => r.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
             .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.HopDongKhamSucKhoeNhanVien)
           .WithMany(r => r.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
           .HasForeignKey(rf => rf.HopDongKhamSucKhoeNhanVienId);

            base.Configure(builder);
        }
    }
}
