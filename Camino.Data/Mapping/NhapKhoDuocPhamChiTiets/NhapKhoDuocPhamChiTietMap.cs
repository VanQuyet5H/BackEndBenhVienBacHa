using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhapKhoDuocPhamChiTiets
{
   public class NhapKhoDuocPhamChiTietMap : CaminoEntityTypeConfiguration<NhapKhoDuocPhamChiTiet>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoDuocPhamChiTietTable);

            builder.HasOne(m => m.NhapKhoDuocPhams)
               .WithMany(u => u.NhapKhoDuocPhamChiTiets)
               .HasForeignKey(m => m.NhapKhoDuocPhamId);

            builder.HasOne(m => m.DuocPhamBenhViens)
               .WithMany(u => u.NhapKhoDuocPhamChiTiets)
               .HasForeignKey(m => m.DuocPhamBenhVienId);

            builder.HasOne(m => m.HopDongThauDuocPhams)
               .WithMany(u => u.NhapKhoDuocPhamChiTiets)
               .HasForeignKey(m => m.HopDongThauDuocPhamId);

            builder.HasOne(m => m.DuocPhamBenhVienPhanNhom)
               .WithMany(u => u.NhapKhoDuocPhamChiTiets)
               .HasForeignKey(m => m.DuocPhamBenhVienPhanNhomId);

            //builder.HasMany(m => m.XuatKhoDuocPhamChiTietViTris)
            //   .WithOne(u => u.NhapKhoDuocPhamChiTiet)
            //   .HasForeignKey(m => m.NhapKhoDuocPhamChiTietId);

            builder.HasOne(m => m.KhoNhapSauKhiDuyet)
               .WithMany(u => u.NhapKhoDuocPhamChiTiets)
               .HasForeignKey(m => m.KhoNhapSauKhiDuyetId);

            builder.HasOne(m => m.NguoiNhapSauKhiDuyet)
                .WithMany(u => u.NhapKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.NguoiNhapSauKhiDuyetId);
            base.Configure(builder);
        }
    }
}
