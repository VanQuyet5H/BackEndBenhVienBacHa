using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhapKhoVatTuMapping
{
    public class NhapKhoVatTuChiTietMap : CaminoEntityTypeConfiguration<NhapKhoVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoVatTuChiTietTable);

            builder.HasOne(rf => rf.NhapKhoVatTu)
                .WithMany(r => r.NhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.NhapKhoVatTuId);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.NhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.HopDongThauVatTu)
                .WithMany(r => r.NhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.HopDongThauVatTuId);

            builder.HasOne(rf => rf.KhoViTri)
                .WithMany(r => r.NhapKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.KhoViTriId);

            builder.HasOne(rf => rf.KhoNhapSauKhiDuyets)
              .WithMany(r => r.NhapKhoVatTuChiTiets)
              .HasForeignKey(rf => rf.HopDongThauVatTuId);

            builder.HasOne(m => m.NguoiNhapSauKhiDuyet)
                .WithMany(u => u.NhapKhoVatTuChiTiets)
                .HasForeignKey(m => m.NguoiNhapSauKhiDuyetId);

            base.Configure(builder);
        }
    }
}
