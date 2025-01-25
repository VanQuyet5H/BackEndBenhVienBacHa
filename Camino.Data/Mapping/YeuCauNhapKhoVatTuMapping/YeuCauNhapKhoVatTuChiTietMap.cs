using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauNhapKhoVatTuMapping
{
    public class YeuCauNhapKhoVatTuChiTietMap : CaminoEntityTypeConfiguration<YeuCauNhapKhoVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhapKhoVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhapKhoVatTuChiTietTable);

            builder.HasOne(rf => rf.YeuCauNhapKhoVatTu)
                .WithMany(r => r.YeuCauNhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.YeuCauNhapKhoVatTuId);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.YeuCauNhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.HopDongThauVatTu)
                .WithMany(r => r.YeuCauNhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.HopDongThauVatTuId);

            builder.HasOne(rf => rf.KhoViTri)
                .WithMany(r => r.YeuCauNhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.KhoViTriId);

            builder.HasOne(m => m.KhoNhapSauKhiDuyet)
                .WithMany(u => u.YeuCauNhapKhoVatTuChiTiets)
                .HasForeignKey(m => m.KhoNhapSauKhiDuyetId);

            builder.HasOne(m => m.NguoiNhapSauKhiDuyet)
                .WithMany(u => u.YeuCauNhapKhoVatTuChiTiets)
                .HasForeignKey(m => m.NguoiNhapSauKhiDuyetId);

            base.Configure(builder);
        }
    }
}
