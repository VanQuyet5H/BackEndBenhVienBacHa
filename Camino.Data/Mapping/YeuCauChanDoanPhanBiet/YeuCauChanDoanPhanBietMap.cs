using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauChanDoanPhanBiet
{
    public class YeuCauChanDoanPhanBietMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhChanDoanPhanBietTable);

            builder.HasOne(rf => rf.ICD)
                  .WithMany(r => r.YeuCauKhamBenhChanDoanPhanBiets)
                  .HasForeignKey(rf => rf.ICDId);
            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.YeuCauKhamBenhChanDoanPhanBiets)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);
            base.Configure(builder);
        }
    }
}