using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NhanVien
{
    public class HoatDongNhanVienMap : CaminoEntityTypeConfiguration<HoatDongNhanVien>
    {
        public override void Configure(EntityTypeBuilder<HoatDongNhanVien> builder)
        {
            builder.ToTable(MappingDefaults.HoatDongNhanVienTable);

            builder.HasOne(m => m.NhanVien)
                .WithMany(u => u.HoatDongNhanViens)
                .HasForeignKey(m => m.NhanVienId);


            builder.HasOne(m => m.PhongBenhVien)
                .WithMany(u => u.HoatDongNhanViens)
                .HasForeignKey(m => m.PhongBenhVienId);

            base.Configure(builder);
        }
    }
}
