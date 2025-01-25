using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauNhapKhoVatTuMapping
{
    public class YeuCauNhapKhoDuocPhamChiTietMap : CaminoEntityTypeConfiguration<YeuCauNhapKhoDuocPhamChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhapKhoDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhapKhoDuocPhamChiTietTable);

            builder.HasOne(rf => rf.YeuCauNhapKhoDuocPham)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamChiTiets)
                .HasForeignKey(rf => rf.YeuCauNhapKhoDuocPhamId);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamChiTiets)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.HopDongThauDuocPham)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamChiTiets)
                .HasForeignKey(rf => rf.HopDongThauDuocPhamId);

            builder.HasOne(rf => rf.DuocPhamBenhVienPhanNhom)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamChiTiets)
                .HasForeignKey(rf => rf.DuocPhamBenhVienPhanNhomId);

            builder.HasOne(rf => rf.KhoViTri)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamChiTiets)
                .HasForeignKey(rf => rf.KhoViTriId);

            builder.HasOne(m => m.KhoNhapSauKhiDuyet)
                .WithMany(u => u.YeuCauNhapKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.KhoNhapSauKhiDuyetId);

            builder.HasOne(m => m.NguoiNhapSauKhiDuyet)
                .WithMany(u => u.YeuCauNhapKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.NguoiNhapSauKhiDuyetId);


            base.Configure(builder);
        }
    }
}
