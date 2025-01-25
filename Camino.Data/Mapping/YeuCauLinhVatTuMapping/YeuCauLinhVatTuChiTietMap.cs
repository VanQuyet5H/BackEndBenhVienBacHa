using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauLinhVatTuMapping
{
    public class YeuCauLinhVatTuChiTietMap : CaminoEntityTypeConfiguration<YeuCauLinhVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauLinhVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauLinhVatTuChiTietTable);

            builder.HasOne(rf => rf.YeuCauLinhVatTu)
                .WithMany(r => r.YeuCauLinhVatTuChiTiets)
                .HasForeignKey(rf => rf.YeuCauLinhVatTuId);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.YeuCauLinhVatTuChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.YeuCauVatTuBenhVien)
               .WithMany(r => r.YeuCauLinhVatTuChiTiets)
               .HasForeignKey(rf => rf.YeuCauVatTuBenhVienId);
            base.Configure(builder);
        }
    }
}
