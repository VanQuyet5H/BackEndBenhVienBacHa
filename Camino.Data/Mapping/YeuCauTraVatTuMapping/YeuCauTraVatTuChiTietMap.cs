using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauTraVatTuMapping
{
    public class YeuCauTraVatTuChiTietMap : CaminoEntityTypeConfiguration<YeuCauTraVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraVatTuChiTietTable);

            builder.HasOne(rf => rf.YeuCauTraVatTu)
                .WithMany(r => r.YeuCauTraVatTuChiTiets)
                .HasForeignKey(rf => rf.YeuCauTraVatTuId);

            builder.HasOne(rf => rf.XuatKhoVatTuChiTietViTri)
                .WithMany(r => r.YeuCauTraVatTuChiTiets)
                .HasForeignKey(rf => rf.XuatKhoVatTuChiTietViTriId);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.YeuCauTraVatTuChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.HopDongThauVatTu)
                .WithMany(r => r.YeuCauTraVatTuChiTiets)
                .HasForeignKey(rf => rf.HopDongThauVatTuId);

            builder.HasOne(rf => rf.KhoViTri)
                .WithMany(r => r.YeuCauTraVatTuChiTiets)
                .HasForeignKey(rf => rf.KhoViTriId);

            base.Configure(builder);
        }
    }
}
