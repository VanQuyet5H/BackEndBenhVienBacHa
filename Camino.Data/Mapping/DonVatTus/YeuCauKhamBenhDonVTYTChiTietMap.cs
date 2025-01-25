using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonVatTus
{
    public class YeuCauKhamBenhDonVTYTChiTietMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhDonVTYTChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhDonVTYTChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhDonVTYTChiTietTable);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(rf => rf.YeuCauKhamBenhDonVTYTChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.NhomVatTu)
                .WithMany(rf => rf.YeuCauKhamBenhDonVTYTChiTiets)
                .HasForeignKey(rf => rf.NhomVatTuId);

            builder.HasOne(rf => rf.YeuCauKhamBenhDonVTYT)
                .WithMany(rf => rf.YeuCauKhamBenhDonVTYTChiTiets)
                .HasForeignKey(rf => rf.YeuCauKhamBenhDonVTYTId);

            base.Configure(builder);
        }
    }
}
