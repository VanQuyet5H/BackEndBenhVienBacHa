using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhapKhoDuocPhams
{
   public class NhapKhoDuocPhamMap : CaminoEntityTypeConfiguration<NhapKhoDuocPham>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoDuocPhamTable);

            builder.HasOne(m => m.KhoDuocPhams)
               .WithMany(u => u.NhapKhoDuocPhams)
               .HasForeignKey(m => m.KhoId);

            builder.HasMany(m => m.NhapKhoDuocPhamChiTiets)
               .WithOne(u => u.NhapKhoDuocPhams)
               .HasForeignKey(m => m.NhapKhoDuocPhamId);
            builder.HasOne(m => m.XuatKhoDuocPham)
               .WithMany(u => u.NhapKhoDuocPhams)
               .HasForeignKey(m => m.XuatKhoDuocPhamId);

            builder.HasOne(rf => rf.NhanVienNhap)
              .WithMany(r => r.NhapKhoDuocPhams)
              .HasForeignKey(rf => rf.NguoiNhapId);

            builder.HasOne(rf => rf.YeuCauNhapKhoDuocPham)
              .WithMany(r => r.NhapKhoDuocPhams)
              .HasForeignKey(rf => rf.YeuCauNhapKhoDuocPhamId);

            builder.HasOne(rf => rf.YeuCauLinhDuocPham)
              .WithMany(r => r.NhapKhoDuocPhams)
              .HasForeignKey(rf => rf.YeuCauLinhDuocPhamId);

            base.Configure(builder);
        }
    }
}
