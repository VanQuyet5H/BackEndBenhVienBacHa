using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhoDuocPhamMapping
{
    public class DuTruDuocPhamMap : CaminoEntityTypeConfiguration<DuTruDuocPham>
    {
        public override void Configure(EntityTypeBuilder<DuTruDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.DuTruDuocPhamTable);

            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.DuTruDuocPhams)
                .HasForeignKey(rf => rf.KhoaPhongId);
            builder.HasOne(rf => rf.NhanVienLapDuTru)
                .WithMany(r => r.DuTruDuocPhams)
                .HasForeignKey(rf => rf.NhanVienLapDuTruId);
            builder.HasOne(rf => rf.DuocPham)
                .WithMany(r => r.DuTruDuocPhams)
                .HasForeignKey(rf => rf.DuocPhamId);

            base.Configure(builder);
        }
    }
}
