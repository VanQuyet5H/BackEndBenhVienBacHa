using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauLinhDuocPhamMapping
{
    public class YeuCauLinhDuocPhamMap : CaminoEntityTypeConfiguration<YeuCauLinhDuocPham>
    {
        public override void Configure(EntityTypeBuilder<YeuCauLinhDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauLinhDuocPhamTable);

            builder.HasOne(rf => rf.KhoXuat)
                .WithMany(r => r.YeuCauLinhDuocPhamKhoXuats)
                .HasForeignKey(rf => rf.KhoXuatId);

            builder.HasOne(rf => rf.KhoNhap)
                .WithMany(r => r.YeuCauLinhDuocPhamKhoNhaps)
                .HasForeignKey(rf => rf.KhoNhapId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.YeuCauLinhDuocPhamNhanVienYeuCaus)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.YeuCauLinhDuocPhamNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            builder.HasOne(rf => rf.NoiYeuCau)
                .WithMany(r => r.YeuCauLinhDuocPhams)
                .HasForeignKey(rf => rf.NoiYeuCauId);

            base.Configure(builder);
        }
    }
}
