using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Thuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Thuocs
{
    public class ADRMap : CaminoEntityTypeConfiguration<ADR>
    {
        public override void Configure(EntityTypeBuilder<ADR> builder)
        {
            builder.ToTable(MappingDefaults.ADRTable);

            builder.HasOne(rf => rf.ThuocHoacHoatChat1)
                .WithMany(r => r.ADR1s)
                .HasForeignKey(rf => rf.ThuocHoacHoatChat1Id);
            builder.HasOne(rf => rf.ThuocHoacHoatChat2)
                .WithMany(r => r.ADR2s)
                .HasForeignKey(rf => rf.ThuocHoacHoatChat2Id);

            base.Configure(builder);
        }
    }
}
