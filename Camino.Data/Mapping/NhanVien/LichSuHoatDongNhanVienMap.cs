using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NhanVien
{
    public class LichSuHoatDongNhanVienMap : CaminoEntityTypeConfiguration<LichSuHoatDongNhanVien>
    {
        public override void Configure(EntityTypeBuilder<LichSuHoatDongNhanVien> builder)
        {
            builder.ToTable(MappingDefaults.LichSuHoatDongNhanVienTable);

            builder.HasOne(m => m.NhanVien)
               .WithMany(u => u.LichSuHoatDongNhanViens)
               .HasForeignKey(m => m.NhanVienId);


            builder.HasOne(m => m.PhongBenhVien)
                .WithMany(u => u.LichSuHoatDongNhanViens)
                .HasForeignKey(m => m.PhongBenhVienId);

            base.Configure(builder);
        }
    }
}
