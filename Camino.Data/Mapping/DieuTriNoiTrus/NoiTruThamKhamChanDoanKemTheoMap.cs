using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruThamKhamChanDoanKemTheoMap : CaminoEntityTypeConfiguration<NoiTruThamKhamChanDoanKemTheo>
    {
        public override void Configure(EntityTypeBuilder<NoiTruThamKhamChanDoanKemTheo> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruThamKhamChanDoanKemTheoTable);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
                .WithMany(r => r.NoiTruThamKhamChanDoanKemTheos)
                .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.ICD)
                .WithMany(r => r.NoiTruThamKhamChanDoanKemTheos)
                .HasForeignKey(rf => rf.ICDId);

            base.Configure(builder);
        }
    }
}
