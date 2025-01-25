using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauTraVatTuTuBenhNhanChiTietMap : CaminoEntityTypeConfiguration<YeuCauTraVatTuTuBenhNhanChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraVatTuTuBenhNhanChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraVatTuTuBenhNhanChiTietTable);

            builder.HasOne(rf => rf.YeuCauTraVatTuTuBenhNhan)
                .WithMany(r => r.YeuCauTraVatTuTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.YeuCauTraVatTuTuBenhNhanId);

            builder.HasOne(rf => rf.YeuCauVatTuBenhVien)
                .WithMany(r => r.YeuCauTraVatTuTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.YeuCauVatTuBenhVienId);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.YeuCauTraVatTuTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.KhoTra)
                .WithMany(r => r.YeuCauTraVatTuTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.KhoTraId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.YeuCauTraVatTuTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            base.Configure(builder);
        }
    }
}
