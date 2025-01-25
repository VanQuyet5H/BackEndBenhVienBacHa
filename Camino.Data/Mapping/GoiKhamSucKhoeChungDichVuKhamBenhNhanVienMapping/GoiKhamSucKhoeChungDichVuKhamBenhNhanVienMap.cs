using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.GoiKhamSucKhoeChungDichVuKhamBenhNhanVienMapping
{
    public class GoiKhamSucKhoeChungDichVuKhamBenhNhanVienMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeChungDichVuKhamBenhNhanVienTable);

            builder.HasOne(rf => rf.GoiKhamSucKhoeDichVuKhamBenh)
              .WithMany(r => r.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
              .HasForeignKey(rf => rf.GoiKhamSucKhoeDichVuKhamBenhId);

            builder.HasOne(rf => rf.GoiKhamSucKhoe)
             .WithMany(r => r.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
             .HasForeignKey(rf => rf.GoiKhamSucKhoeId);

            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
             .WithMany(r => r.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
             .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);

            builder.HasOne(rf => rf.HopDongKhamSucKhoeNhanVien)
           .WithMany(r => r.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
           .HasForeignKey(rf => rf.HopDongKhamSucKhoeNhanVienId);

            base.Configure(builder);
        }
    }
}
