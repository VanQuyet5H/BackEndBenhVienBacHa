using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauTraDuocPhamMapping
{
    public class YeuCauTraDuocPhamChiTietMap : CaminoEntityTypeConfiguration<YeuCauTraDuocPhamChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraDuocPhamChiTietTable);

            builder.HasOne(rf => rf.YeuCauTraDuocPham)
                .WithMany(r => r.YeuCauTraDuocPhamChiTiets)
                .HasForeignKey(rf => rf.YeuCauTraDuocPhamId);

            builder.HasOne(rf => rf.XuatKhoDuocPhamChiTietViTri)
                .WithMany(r => r.YeuCauTraDuocPhamChiTiets)
                .HasForeignKey(rf => rf.XuatKhoDuocPhamChiTietViTriId);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.YeuCauTraDuocPhamChiTiets)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.KhoViTri)
                .WithMany(r => r.YeuCauTraDuocPhamChiTiets)
                .HasForeignKey(rf => rf.KhoViTriId);

            builder.HasOne(rf => rf.HopDongThauDuocPham)
                .WithMany(r => r.YeuCauTraDuocPhamChiTiets)
                .HasForeignKey(rf => rf.HopDongThauDuocPhamId);

            builder.HasOne(rf => rf.DuocPhamBenhVienPhanNhom)
                .WithMany(r => r.YeuCauTraDuocPhamChiTiets)
                .HasForeignKey(rf => rf.DuocPhamBenhVienPhanNhomId);

            base.Configure(builder);
        }
    }
}
