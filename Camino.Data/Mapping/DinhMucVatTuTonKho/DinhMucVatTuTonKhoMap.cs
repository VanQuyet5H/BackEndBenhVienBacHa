using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DinhMucVatTuTonKho
{
    public class DinhMucVatTuTonKhoMap : CaminoEntityTypeConfiguration<Camino.Core.Domain.Entities.DinhMucVatTuTonKhos.DinhMucVatTuTonKho>
    {
        public override void Configure(EntityTypeBuilder<Camino.Core.Domain.Entities.DinhMucVatTuTonKhos.DinhMucVatTuTonKho> builder)
        {
            builder.ToTable(MappingDefaults.DinhMucVatTuTonKhoTable);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.DinhMucVatTuTonKhos)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.Kho)
                .WithMany(r => r.DinhMucVatTuTonKhos)
                .HasForeignKey(rf => rf.KhoId);

            base.Configure(builder);
        }
    }
}
